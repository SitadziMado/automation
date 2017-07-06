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
    public class Message
    {
        public Message(MessageType type, byte[] rawData)
        {
            Init(type, rawData);
        }

        public Message(MessageType type, string stringData)
        {
            if (stringData == null)
                throw new ArgumentNullException();

            Init(type, Encoding.UTF8.GetBytes(stringData));
        }

        public static implicit operator byte[](Message lhs)
        {
            return lhs.Buffer;
        }

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

        private void Init(MessageType type, byte[] rawData)
        {
            if (type == MessageType.None && rawData == null)
                throw new ArgumentNullException();

            m_type = type;
            // var msg = Encoding.UTF8.GetBytes(GetMessageString(m_type));
            m_rawData = new byte[rawData.Length];
            // m_rawData = new byte[msg.Length + rawData.Length];
            // Array.Copy(msg, m_rawData, msg.Length);
            // Array.Copy(rawData, 0, m_rawData, msg.Length, rawData.Length);
            Array.Copy(rawData, m_rawData, rawData.Length);
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

        public byte[] Buffer { get { return m_rawData; } }
        public MessageType MsgType { get { return m_type; } }

        private MessageType m_type = MessageType.None;
        private byte[] m_rawData = null;
    }
}
