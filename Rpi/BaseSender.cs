using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
            Logger.WriteLine(this, "Инициализация клиента на порте {0}", this.port);
        }

        /// <summary>
        /// Отсылка строки клиенту.
        /// </summary>
        /// <param name="client">Клиент, которому отправляется сообщение.</param>
        /// <param name="msg">Строка-сообщение.</param>
        /// <param name="parameters">Параметры, передаваемые клиенту.</param>
        /// <returns>Истина, если успешно.</returns>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="IOException"></exception>
        protected bool SendStringToClient(TcpClient client, string msg, params object[] parameters)
        {
            Logger.WriteLine(this, "Начало передачи сообщения");

            if (client == null)
            {
                Logger.WriteLine(this, "Нет подключения");
                return false;
            }

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
                    Logger.WriteLine(this, "Отправляю сообщение: {0}", sb);
                    sw.Write(sb.ToString());

                    // Получаем подтверждение о получении.
                    var reply = sr.ReadToEnd();
                    Logger.WriteLine(this, "Получено подтверждение: {0}", reply);

                    // Сравниваем с тем, что ожидаем.
                    if (reply != Message.Ack)
                    {
                        Logger.WriteLine(this, "Подтверждение неверно, ошибка");
                        return false;
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

            Logger.WriteLine(this, "Отправление успешно");
            return true;
        } // SendString

        /// <summary>
        /// Порт, по которому действует класс.
        /// </summary>
        protected int port;
    }
}
