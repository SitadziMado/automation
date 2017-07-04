using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    /// <summary>
    /// Класс сервера, принимающего клиентов.
    /// </summary>
    public class Server
    {
        /// <summary>
        /// Конструктор по умолчанию для сервера.
        /// </summary>
        /// <param name="port">Порт для принятия клиентов.</param>
        public Server(int port)
        {
            m_port = port;
        } // Server

        /// <summary>
        /// Начать работу данного сервераю
        /// </summary>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="IOException"></exception>
        public void Start()
        {
            m_listen = true;

            Thread thread = new Thread(() =>
            {
                IPAddress ip;
                TcpListener server = null;

                try
                {
                    ip = new IPAddress(127 | 0 | 0 | 1 << 24);
                    server = new TcpListener(ip, m_port);
                    server.Start();

                    while (m_listen)
                    {
                        if (!server.Pending())
                        {
                            Thread.Sleep(PendingCooldown);
                            continue;
                        }

                        var client = server.AcceptTcpClient();
                        var address = ((IPEndPoint)client.Client.RemoteEndPoint).Address;

                        if (!m_addresses.Contains(address))
                        {
                            using (var sw = new StreamWriter(client.GetStream()))
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
                    // Останавливаем сервер и уничтожаем клиентов.
                    server?.Stop();
                    m_clients.Clear();
                    m_addresses.Clear();
                }
            });
        } // Start

        /// <summary>
        /// Завершить работу данного сервера.
        /// </summary>
        public void Stop()
        {
            m_listen = false;
        } // Stop

        /// <summary>
        /// Отсылка строки клиенту.
        /// </summary>
        /// <param name="clientId">Идентификатор клиента.</param>
        /// <param name="msg">Строка-сообщение.</param>
        /// <param name="parameters">Параметры, передаваемые клиенту.</param>
        /// <returns>Истина, если успешно.</returns>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="IOException"></exception>
        public bool SendString(int clientId, string msg, params object[] parameters)
        {
            if (clientId < 0 || clientId >= m_clients.Count)
                return false;

            var client = m_clients[clientId];

            if (client == null)
                return false;

            try
            {
                var stream = client.GetStream();

                using (var sr = new StreamReader(stream))
                using (var sw = new StreamWriter(stream))
                {
                    // Начало формирования сообщения.
                    var sb = new StringBuilder(msg).Append(" ");

                    // Добавление к сообщению всех его параметров.
                    foreach (var v in parameters)
                        sb.AppendFormat(" {0}", v);

                    // Прописываем сообщение клиенту.
                    sw.Write(sb.ToString());

                    // Получаем подтверждение о получении.
                    var reply = sr.ReadToEnd();

                    // Сравниваем с тем, что ожидаем.
                    if (reply != Message.Ack)
                        return false;
                }
            }
            catch (SocketException e)
            {
                // Удаляем адрес и клиента.
                m_addresses.Remove(((IPEndPoint)client.Client.RemoteEndPoint).Address);
                m_clients.RemoveAt(clientId);
                return false;

                // throw e;
            }
            catch (IOException e)
            {
                throw e;
            }

            return true;
        } // SendString

        private const int PendingCooldown = 500;
        private const int DefaultReceiveTimeout = 10000;

        /// <summary>
        /// Количество клиентов на данный момент.
        /// </summary>
        public int Count { get { return m_clients.Count; } }

        private List<TcpClient> m_clients = new List<TcpClient>();
        private HashSet<IPAddress> m_addresses = new HashSet<IPAddress>();
        
        private bool m_listen = false;
        private int m_port;
    }
}
