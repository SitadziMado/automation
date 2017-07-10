using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rpi
{
    /// <summary>
    /// Типы сообщений, возможных для проекта.
    /// </summary>
    public enum MessageType
    {
        None,
        Greet,
        Ack,
        RequestIds,
        RequestData,
        SendIds,
        SendData,
        Finish,
    }
}
