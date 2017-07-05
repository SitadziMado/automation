using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rpi
{
    /// <summary>
    /// Класс, содержащий определения для сообщений между клиентом и серевером.
    /// </summary>
    public static class Message
    {
        /// <summary>
        /// Запрос на добавление нового клиента к серверу.
        /// </summary>
        public const string Greet = "greet";

        /// <summary>
        /// Подтверждение приема.
        /// </summary>
        public const string Ack = "ack";

        /// <summary>
        /// Запрос доступных идентификаторов.
        /// </summary>
        public const string RequestIds = "ireq";

        /// <summary>
        /// Запрос данных у сервера.
        /// </summary>
        public const string RequestData = "dreq";

        /// <summary>
        /// Отправка идентификаторов.
        /// </summary>
        public const string SendIds = "ids";

        /// <summary>
        /// Отправка информации.
        /// </summary>
        public const string SendData = "data";
    }
}
