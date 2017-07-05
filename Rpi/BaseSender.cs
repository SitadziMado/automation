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
    /// Базовый класс для клиента и сервера.
    /// </summary>
    public abstract class BaseSender
    {
        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public BaseSender(int port)
        {
            this.port = port;
            Logger.WriteLine(this, "Инициализация на порте {0}", this.port);
        }

        /// <summary>
        /// Отсылка строки клиенту.
        /// </summary>
        /// <param name="client">Клиент, которому отправляется сообщение.</param>
        /// <param name="msg">Строка-сообщение.</param>
        /// <param name="parameters">Параметры, передаваемые клиенту.</param>
        /// <returns>Сообщение при успехе, иначе null.</returns>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="IOException"></exception>
        protected string SendStringToClient(TcpClient client, string msg, params object[] parameters)
        {
            Logger.WriteLine(this, "Начало передачи сообщения");

            if (client == null)
            {
                Logger.WriteLine(this, "Нет подключения");
                return null;
            }

            StreamReader sr = null;
            StreamWriter sw = null;

            try
            {
                var stream = client.GetStream();

                sr = new StreamReader(stream);
                sw = new StreamWriter(stream);
                // using (var sr = new StreamReader(stream))
                // using (var sw = new StreamWriter(stream))
                {
                    // Начало формирования сообщения.
                    var sb = new StringBuilder(msg).Append(" ");

                    // Добавление к сообщению всех его параметров.
                    foreach (var v in parameters)
                        sb.AppendFormat(" {0}", v);

                    // Прописываем сообщение клиенту.
                    Logger.WriteLine(this, "Отправляю сообщение: {0}", sb);
                    sw.WriteLine(sb.ToString());

                    // Получаем подтверждение о получении.
                    var reply = sr.ReadLine();
                    Logger.WriteLine(this, "Получен ответ: {0}", reply);

                    // Сравниваем с тем, что ожидаем.
                    /*if (reply != Message.Ack)
                    {
                        Logger.WriteLine(this, "Подтверждение неверно, ошибка");
                        return false;
                    }*/

                    Logger.WriteLine(this, "Отправление успешно");
                    return reply;
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
            catch (InvalidOperationException e)
            {
                throw e;
            }
            finally
            {
                sr?.Close();
                sw?.Close();
            }
        } // SendString

        /// <summary>
        /// Порт, по которому действует класс.
        /// </summary>
        protected int port;
        protected bool listen = false;
    }
}
