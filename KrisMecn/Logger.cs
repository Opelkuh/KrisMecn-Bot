using KrisMecn.Enums;
using System;

namespace KrisMecn
{
    static class Logger
    {
        public static void Info(string message) => Log(LogLevel.Info, message);
        public static void Info(object message) => Log(LogLevel.Info, message);

        public static void Debug(string message) => Log(LogLevel.Debug, message);
        public static void Debug(object message) => Log(LogLevel.Debug, message);

        public static void Warn(string message) => Log(LogLevel.Warn, message);
        public static void Warn(object message) => Log(LogLevel.Warn, message);

        public static void Error(string message) => Log(LogLevel.Error, message);
        public static void Error(object message) => Log(LogLevel.Error, message);

        public static void Log(LogLevel level, object message)
            => Log(level, message.ToString());
        public static void Log(LogLevel level, string message)
        {
            Console.Write("[");
            printLevel(level);
            Console.Write("] ");
            Console.Write(DateTime.Now.ToString());
            Console.Write("  -  ");
            Console.Write(message);
            Console.Write("\n");
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
