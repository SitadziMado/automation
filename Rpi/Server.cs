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
        public Server(int port, ClientProc clientProc) :
            base(port)
        {
            if (clientProc == null)
                throw new ArgumentNullException(
                    "Укажите ненулевой делегат процедуры клиента."
                );

            m_clientProc = clientProc;
        } // Server

        /// <summary>
        /// Деструктор.
        /// </summary>
        ~Server()
        {
            // Logger.WriteLine(this, "Уничтожение сервера");
        }

        /// <summary>
        /// Начать работу данного сервера.
        /// </summary>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="IOException"></exception>
        public void Start()
        {
            Logger.WriteLine(this, "Начало работы сервера");
            listen = true;

            Thread listenerThread = new Thread(() =>
            {
                IPAddress ip;
                TcpListener server = null;

                try
                {
                    // Подключаемся к адресу по умолчанию.
                    ip = new IPAddress(127 | 0 | 0 | 1 << 24);
                    server = new TcpListener(ip, port);
                    server.Start();
                    Logger.WriteLine(this, "Сервер начал прослушивание на порте {0}", port);

                    while (listen)
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
            // listenerThread.Start();

            Thread pollThread = new Thread(() =>
            {
                while (listen)
                {
                    for (int i = 0; i < m_clients.Count; ++i)
                    {
                        var c = m_clients[i];

                        // Если есть данные, то обрабатываем запрос.
                        if (c.Available > 0)
                        {
                            // Берем стрим.
                            var stream = c.GetStream();
                            var sr = new StreamReader(stream);
                            var sw = new StreamWriter(stream);

                            // Получаем запрос
                            var str = sr.ReadLine().Trim();
                            var req = str.Split(' ');
                            Logger.WriteLine(this, "Принято сообщение: `{0}`", str);

                            // Вызываем процедуру обработки запроса.
                            // ToDo: делать это асинхронно!!!!!!!!!!!!!!!
                            Logger.WriteLine(this, "Вызывается процедура обработки", str);
                            var data = m_clientProc(i, Message.GetMessageType(req[0]), req.Skip(1).ToArray());
                            stream.Write(data, 0, data.Length);

                            // sr.Close();
                            // sw.Close();
                        }
                    }

                    Thread.Sleep(PendingCooldown);
                }
            });
            pollThread.Start();
        } // Start

        /// <summary>
        /// Завершить работу данного сервера.
        /// </summary>
        public void Stop()
        {
            Logger.WriteLine(this, "Запрос на остановку работы сервера");
            listen = false;
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
        [Obsolete("Не использовать этот метод.")]
        public bool SendString(int clientId, MessageType msg, params object[] parameters)
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
                return SendStringToClient(client, msg, parameters) != null;
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
                Logger.WriteLine(
                    this, 
                    "Попытка присоединиться к адресу {0}:{1}",
                    hostname,
                    port
                );

                var client = new TcpClient(hostname, port);
                // using (var client = new TcpClient(hostname, port))
                {
                    client.Client.SendTimeout = DefaultSendTimeout;
                    client.Client.ReceiveTimeout = DefaultReceiveTimeout;

                    // Берем стрим.
                    var stream = client.GetStream();

                    var sr = new StreamReader(stream);
                    var sw = new StreamWriter(stream);
                    // using (var sr = new StreamReader(stream))
                    // using (var sw = new StreamWriter(stream))
                    {
                        // Пишем приветствие клиенту.
                        Logger.WriteLine(this, "Отправка приветствия");
                        sw.WriteLine(Message.Greet);
                        sw.Flush();

                        // Ожидаем ответ.
                        var reply = sr.ReadLine();
                        Logger.WriteLine(this, "Ответ получен: `{0}`", reply);

                        // Если ответ не тот, то игнорируем клиента.
                        if (reply != Message.Ack)
                        {
                            Logger.WriteLine(this, "Подтверждение неверно, ошибка");
                            return false;
                        }

                        // Принимаем клиента при наличии подключения.
                        var address = ((IPEndPoint)client.Client.RemoteEndPoint).Address;

                        // Если клиент новый, то добавляем его.
                        if (!m_addresses.Contains(address))
                        {
                            // Отправляем подтверждение о подключении.
                            Logger.WriteLine(this, "Отправляем подтверждение о принятии подключения");
                            // using (var sw = new StreamWriter(client.GetStream()))
                            sw.Write(Message.Ack);

                            client.SendTimeout = DefaultSendTimeout;
                            client.ReceiveTimeout = DefaultReceiveTimeout;
                            m_addresses.Add(address);
                            m_clients.Add(client);
                        }
                    }
                }
            }
            catch (SocketException /*e*/)
            {
                Logger.WriteLine(this, "Попытка соединения оказалась неудачной");
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

        /// <summary>
        /// Список адресов клиентов.
        /// </summary>
        public string[] ClientAddressses
        {
            get
            {
                var list = new List<string>();
                foreach (var v in m_addresses)
                    list.Add(v.ToString());
                return list.ToArray();
            }
        }

        /// <summary>
        /// Истина, если сервер работает в данный момент.
        /// </summary>
        public bool On { get { return listen; } }

        private List<TcpClient> m_clients = new List<TcpClient>();
        private HashSet<IPAddress> m_addresses = new HashSet<IPAddress>();

        private ClientProc m_clientProc = null;
    }

    public delegate byte[] ClientProc(int id, MessageType msg, object[] parameters);
}
