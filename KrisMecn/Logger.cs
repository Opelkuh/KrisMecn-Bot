using KrisMecn.Enums;
using System;
using System.Text;
using System.Threading;

namespace KrisMecn
{
    static class Logger
    {
        private static Mutex _mutex = new Mutex();

        public static void Info(string message) => Log(LogLevel.Info, message);
        public static void Info(object message) => Log(LogLevel.Info, message);
        public static void Info(string message, object data) => Log(LogLevel.Info, message, data);

        public static void Debug(string message) => Log(LogLevel.Debug, message);
        public static void Debug(object message) => Log(LogLevel.Debug, message);
        public static void Debug(string message, object data) => Log(LogLevel.Debug, message, data);

        public static void Warn(string message) => Log(LogLevel.Warn, message);
        public static void Warn(object message) => Log(LogLevel.Warn, message);
        public static void Warn(string message, object data) => Log(LogLevel.Warn, message, data);

        public static void Error(string message) => Log(LogLevel.Error, message);
        public static void Error(object message) => Log(LogLevel.Error, message);
        public static void Error(string message, object data) => Log(LogLevel.Error, message, data);

        public static void Log(LogLevel level, object message)
            => Log(level, message.ToString());
        public static void Log(LogLevel level, string message, object data = null)
        {
            // prepare log string
            var sb = new StringBuilder()
                .Append(DateTime.Now.ToString())
                .Append("  -  ")
                .Append(message);

            // add optional data
            if (data != null)
            {
                sb.Append(" - ")
                  .Append(data.ToString());
            }

            // wait for write permission
            _mutex.WaitOne();

            // print everything
            Console.Write("[");
            printLevel(level);
            Console.Write("] ");

            Console.WriteLine(sb.ToString());

            // release mutex
            _mutex.ReleaseMutex();
        }

        private static void printLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("DEBUG");
                    break;
                case LogLevel.Info:
                    Console.Write("INFO");
                    break;
                case LogLevel.Warn:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("WARN");
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("ERROR");
                    break;
            }

            Console.ResetColor();
        }
    }
}
