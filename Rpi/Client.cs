using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            Logger.WriteLine(this, "Уничтожение сервера");
        }

        /// <summary>
        /// Отсылка строки клиенту.
        /// </summary>
        /// <param name="msg">Строка-сообщение.</param>
        /// <param name="parameters">Параметры, передаваемые клиенту.</param>
        /// <returns>Истина, если успешно.</returns>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="IOException"></exception>
        public bool SendString(string msg, params object[] parameters)
        {
            try
            {
                return SendStringToClient(m_client, msg, parameters);
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
        /// Ждать до подключения асинхронно. По подключении устройство имеет адрес сервера.
        /// </summary>
        public void AsyncWaitForConnection()
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

                    while (true)
                    {
                        if (!server.Pending())
                        {
                            Thread.Sleep(PendingCooldown);
                            continue;
                        }

                        // Слушаем клиента, если он есть
                        var client = server.AcceptTcpClient();

                        // Устанавливаем тайм-ауты, чтобы при ошибке соединение разорвать.
                        client.SendTimeout = DefaultSendTimeout;
                        client.ReceiveTimeout = DefaultReceiveTimeout;

                        // Адрес клиента, понадобится позже.
                        var endPoint = ((IPEndPoint)client.Client.RemoteEndPoint);

                        // Берем стрим.
                        var stream = client.GetStream();

                        using (var sr = new StreamReader(stream))
                        using (var sw = new StreamWriter(stream))
                        {
                            // Прочитать приветствие.
                            var msg = sr.ReadToEnd();

                            // Если это не приветствие, то это не наш клиент.
                            if (!(msg == Message.Greet))
                            {
                                client.Close();
                                continue;
                            }

                            // Ответ на приветствие.
                            sw.Write(Message.Ack);

                            // Сохранить адрес сервера.
                            m_endPoint = endPoint;
                            break;
                        }
                    }

                    // Подключаемся к серверу.
                    m_client = new TcpClient(m_endPoint);
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

        private const int PendingCooldown = 500;
        private const int DefaultSendTimeout = 10000;
        private const int DefaultReceiveTimeout = 10000;

        private IPEndPoint m_endPoint = null;
        private TcpClient m_client = null;
    }
}
