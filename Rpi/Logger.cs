using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public static class Logger
    {
        public static void CreateLogFile(string filename = null, bool append = true)
        {
            if (filename == null || filename == "")
            {
                var dt = DateTime.Now;
                filename = new StringBuilder(LogPrefix)
                    .Append(dt.ToShortDateString().Replace('.', '-'))
                    .Append("_")
                    .Append(dt.ToShortTimeString().Replace('.', '-'))
                    .Append(".txt")
                    .ToString();
            }
            try
            {
                file = new StreamWriter(filename, append);
            }
            catch (DirectoryNotFoundException e)
            {
                throw e;
            }
            catch (IOException e)
            {
                throw e;
            }
        }

        public static void WriteLine(object obj, string msg)
        {
            try
            {
                var dt = DateTime.Now;
                var s = new StringBuilder()
                    .AppendFormat(
                        "{0} {1} ({2}): {3}.", 
                        dt.ToShortDateString(), 
                        dt.ToShortTimeString(), 
                        obj.ToString(), 
                        msg
                    )
                    .ToString();
                log.AppendLine(s);
                file?.WriteLine(s);
            }
            catch (IOException e)
            {
                throw e;
            }
        }

        public static string GetLogString()
        {
            return log.ToString();
        }

        private const string LogPrefix = @"/logs/log_";

        private static StreamWriter file = null;
        private static StringBuilder log = new StringBuilder();
    }
}
