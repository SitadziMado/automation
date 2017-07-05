using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Rpi
{
    /// <summary>
    /// Класс сервера, принимающего клиентов.
    /// </summary>
    public class Server : BaseSender
    {
        /// <summary>
        /// Конструктор по умолчанию для сервера.
        /// </summary>
        /// <param name="port">Порт для принятия клиентов.</param>
        public Server(int port) :
            base(port)
        {
        } // Server

        /// <summary>
        /// Деструктор.
        /// </summary>
        ~Server()
        {
            Logger.WriteLine(this, "Уничтожение сервера");
        }

        /// <summary>
        /// Начать работу данного сервера.
        /// </summary>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="IOException"></exception>
        public void Start()
        {
            Logger.WriteLine(this, "Начало работы сервера");
            m_listen = true;

            Thread thread = new Thread(() =>
            {
                IPAddress ip;
                TcpListener server = null;

                try
                {
                    // Подключаемся к адресу по умолчанию.
                    ip = new IPAddress(127 | 0 | 0 | 1 << 24);
                    server = new TcpListener(ip, port);
                    server.Start();
                    Logger.WriteLine(this, "Сервер начал прослушивание на порту {0}", port);

                    while (m_listen)
                    {
                        // Проверяем, есть ли доступные подключения.
                        if (!server.Pending())
                        {
                            Thread.Sleep(PendingCooldown);
                            continue;
                        }

                        // Принимаем клиента при наличии подключения.
                        var client = server.AcceptTcpClient();
                        var address = ((IPEndPoint)client.Client.RemoteEndPoint).Address;
                        Logger.WriteLine(this, "Входящее подключение по адресу {0}", address.ToString());

                        // Если клиент новый, то добавляем его.
                        if (!m_addresses.Contains(address))
                        {
                            // Отправляем подтверждение о подключении.
                            Logger.WriteLine(this, "Отправляем подтверждение о принятии подключения");
                            // using (var sw = new StreamWriter(client.GetStream()))
                            var sw = new StreamWriter(client.GetStream());
                            sw.Write(Message.Ack);

                            client.ReceiveTimeout = DefaultReceiveTimeout;
                            m_addresses.Add(address);
                            m_clients.Add(client);
                        }
                    }
                }
                catch (SocketException e)
                {
                    throw e;
                }
                catch (IOException e)
                {
                    throw e;
                }
                finally
                {
                    // ToDo: сделать реконнект.
                    // Останавливаем сервер и уничтожаем клиентов.
                    Logger.WriteLine(this, "Остановка работы сервера");
                    server?.Stop();
                    m_clients.Clear();
                    m_addresses.Clear();
                }
            });

            thread.Start();
        } // Start

        /// <summary>
        /// Завершить работу данного сервера.
        /// </summary>
        public void Stop()
        {
            Logger.WriteLine(this, "Запрос на остановку работы сервера");
            m_listen = false;
        } // Stop

        /// <summary>
        /// Отсылка строки клиенту.
        /// </summary>
        /// <param name="clientId">Клиент, которому отправляется сообщение.</param>
        /// <param name="msg">Строка-сообщение.</param>
        /// <param name="parameters">Параметры, передаваемые клиенту.</param>
        /// <returns>Истина, если успешно.</returns>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="IOException"></exception>
        public bool SendString(int clientId, string msg, params object[] parameters)
        {
            if (clientId < 0 || clientId >= m_clients.Count)
            {
                Logger.WriteLine(this, "Пока нет доступных клиентов");
                return false;
            }

            Logger.WriteLine(this, "Клиент №{0} выбран", clientId);
            var client = m_clients[clientId];

            try
            {
                return SendStringToClient(client, msg, parameters);
            }
            catch (SocketException e)
            {
                // ToDo: добавить реконнект и разрыв соединения.
                throw e;
            }
            catch (IOException e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Запросить соединение у нового клиента.
        /// </summary>
        /// <param name="hostname">Адрес клиента.</param>
        /// <param name="port">Порт, к которому нужно подключиться.</param>
        /// <returns>Истина, если подключение успешно.</returns>
        public bool AddDevice(string hostname, int port)
        {
            try
            {
                using (var client = new TcpClient(hostname, port))
                {
                    client.Client.SendTimeout = DefaultSendTimeout;
                    client.Client.ReceiveTimeout = DefaultReceiveTimeout;

                    // Берем стрим.
                    var stream = client.GetStream();

                    using (var sr = new StreamReader(stream))
                    using (var sw = new StreamWriter(stream))
                    {
                        // Пишем приветствие клиенту.
                        Logger.WriteLine(this, "Отправка приветствия");
                        sw.WriteLine(Message.Greet);
                        sw.Flush();

                        // Ожидаем ответ.
                        var reply = sr.ReadLine();
                        Logger.WriteLine(this, "Ответ получен: {0}", reply);

                        // Если ответ не тот, то игнорируем клиента.
                        if (reply != Message.Ack)
                        {
                            Logger.WriteLine(this, "Подтверждение неверно, ошибка");
                            return false;
                        }
                    }
                }
            }
            catch (SocketException /*e*/)
            {
                return false;
            }

            return true;
        }

        private const int PendingCooldown = 500;
        private const int DefaultSendTimeout = 10000 * 10;
        private const int DefaultReceiveTimeout = 10000 * 10;

        /// <summary>
        /// Количество клиентов на данный момент.
        /// </summary>
        public int Count { get { return m_clients.Count; } }

        private List<TcpClient> m_clients = new List<TcpClient>();
        private HashSet<IPAddress> m_addresses = new HashSet<IPAddress>();
        
        private bool m_listen = false;
    }
}
