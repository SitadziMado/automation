﻿#define LOCAL

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
    /// Класс клиента.
    /// </summary>
    public class Client : BaseSender
    {
        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        /// <param name="port">Порт, по которому будет происходить общение с сервером.</param>
        public Client(int port) :
            base(port)
        {
        }

        /// <summary>
        /// Деструктор.
        /// </summary>
        ~Client()
        {
            // Logger.WriteLine(this, "Уничтожение сервера");
        }

        /// <summary>
        /// Начать работу клиента.
        /// </summary>
        public void Start()
        {
            Logger.WriteLine(this, "Клиент запускается");
            listen = true;
            AsyncWaitForConnection();
        }

        /// <summary>
        /// Остановить работу клиента.
        /// </summary>
        public void Stop()
        {
            Logger.WriteLine(this, "Запрос на завершение работы клиента");
            listen = false;
        }

        /// <summary>
        /// Отсылка строки клиенту.
        /// </summary>
        /// <param name="msg">Строка-сообщение.</param>
        /// <param name="parameters">Параметры, передаваемые клиенту.</param>
        /// <returns>Истина, если успешно.</returns>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="IOException"></exception>
        public string SendString(
            MessageType msg, 
            bool receiveReply = true,
            params object[] parameters
        )
        {
            try
            {
                return SendStringToClient(
                    m_client, 
                    msg, 
                    receiveReply, 
                    parameters
                );
            }
            catch (SocketException e)
            {
                // throw e;
                DropConnection();
            }
            catch (IOException e)
            {
                // throw e;
                DropConnection();
            }

            return null;
        }

        /// <summary>
        /// Хотфикс для притягивания байтов с сервера для .tar.gz.
        /// </summary>
        /// <returns>Массив байтов, представляющих упаковку информации.</returns>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="IOException"></exception>
        public byte[] PullBytes()
        {
            try
            {
                int size = m_client.Client.ReceiveBufferSize;
                byte[] buffer = new byte[size];
                List<byte> data = new List<byte>();

                var s = m_client.GetStream();

                int waited = 0;

                while (waited < DefaultReceiveTimeout && data.Count == 0)
                {
                    Thread.Sleep(PendingCooldown);
                    waited += PendingCooldown;

                    while (m_client.Available > 0)
                    {
                        int length = Math.Min(size, m_client.Available);
                        s.Read(buffer, 0, length);
                        data.AddRange(buffer.Take(length));
                    }
                }

                if (data.Count == 0)
                    throw new IOException("Не было получено ни одного байта");

                return data.ToArray();
            }
            catch (SocketException e)
            {
                // throw e;
                DropConnection();
            }
            catch (IOException e)
            {
                // throw e;
                DropConnection();
            }

            return null;
        }

        /// <summary>
        /// Ждать до подключения асинхронно. По подключении устройство имеет адрес сервера.
        /// </summary>
        private void AsyncWaitForConnection()
        {
            Thread thread = new Thread(() =>
            {
                IPAddress ip;
                TcpListener server = null;

                try
                {
                    ip = new IPAddress(127 | 0 | 0 | 1 << 24);
                    server = new TcpListener(ip, port);
                    server.Start();
                    Logger.WriteLine(this, "Начато ожидание сервера");

                    while (listen)
                    {
                        if (!server.Pending())
                        {
                            Thread.Sleep(PendingCooldown);
                            continue;
                        }

                        // Слушаем клиента, если он есть
                        var client = server.AcceptTcpClient();
                        Logger.WriteLine(this, "Принято возможное приветствие от сервера");

                        // Устанавливаем тайм-ауты, чтобы при ошибке соединение разорвать.
                        client.SendTimeout = DefaultSendTimeout;
                        client.ReceiveTimeout = DefaultReceiveTimeout;

                        // Адрес клиента, понадобится позже.
                        var endPoint = ((IPEndPoint)client.Client.RemoteEndPoint);

                        // Берем стрим.
                        var stream = client.GetStream();

                        var sr = new StreamReader(stream);
                        var sw = new StreamWriter(stream);
                        // using (var sr = new StreamReader(stream))
                        // using (var sw = new StreamWriter(stream))
                        {
                            // Прочитать приветствие.
                            bool av = stream.DataAvailable;
                            var msg = sr.ReadLine();
                            Logger.WriteLine(this, "Прочитано приветствие: `{0}`", msg);

                            // Если это не приветствие, то это не наш клиент.
                            if (!(msg == Message.Greet))
                            {
                                Logger.WriteLine(this, "Неверное приветствие, ошибка", msg);
                                client.Close();
                                continue;
                            }

                            // Ответ на приветствие.
                            Logger.WriteLine(this, "Отправлен ответ на приветствие");
                            sw.WriteLine(Message.Ack);
                            sw.Flush();

                            // Сохранить адрес сервера.
                            m_endPoint = endPoint;
                            
                            // Подключаемся к серверу.
                            m_client = client;
                            break;
                        }
                    }

/*#if LOCAL
                    // m_endPoint.Port = 6400;
                    // m_endPoint.Address = IPAddress.Parse("192.168.137.47");
                    m_client = new TcpClient();
                    m_client.Connect(m_endPoint);
                    bool ok = m_client.Connected;
#else
                    m_client = new TcpClient(m_endPoint);
                    Logger.WriteLine(this, "Установлено подключение к серверу: {0}", m_endPoint.ToString());
#endif*/
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
                    // Останавливаем прослушивание.
                    server?.Stop();
                }
            });
            thread.Start();
        }
        
        /// <summary>
        /// Оборвать соединение и начать прослушивание.
        /// </summary>
        private void DropConnection()
        {
            Logger.WriteLine(this, "Соединение разорвано");
            m_client = null;
            m_endPoint = null;
            AsyncWaitForConnection();
        }

        private const int PendingCooldown = 500;
        private const int DefaultSendTimeout = 10000 * 10;
        private const int DefaultReceiveTimeout = 10000 * 10;

        /// <summary>
        /// Истина, если предыдущее сообщение было доставлено успешно.
        /// </summary>
        public bool Connected { get { return m_client != null; } }

        /// <summary>
        /// Адрес сервера.
        /// </summary>
        public string ServerAddress { get { return m_endPoint.ToString(); } }

        /// <summary>
        /// Истина, если клиент в данный момент активен.
        /// </summary>
        public bool On { get { return listen; } }

        private IPEndPoint m_endPoint = null;
        private TcpClient m_client = null;
    }
}
