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
        public static string GetMessageString(MessageType type)
        {
            if (message.ContainsKey(type))
                return message[type];
            else
                return null;
        }

        public static MessageType GetMessageType(string msg)
        {
            msg = msg.ToLower();
            if (messageInv.ContainsKey(msg))
                return messageInv[msg];
            else
                return MessageType.None;
        }

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

        private static Dictionary<MessageType, string> message = new Dictionary<MessageType, string>()
        {
            { MessageType.Greet, Greet },
            { MessageType.Ack, Ack },
            { MessageType.RequestIds, RequestIds },
            { MessageType.RequestData, RequestData },
            { MessageType.SendIds, SendIds },
            { MessageType.SendData, SendData },
        };
        private static Dictionary<string, MessageType> messageInv = new Dictionary<string, MessageType>(
            (
            from v
            in message
            where true
            select v
            ).ToDictionary(x => x.Value, x => x.Key));
    }
}
