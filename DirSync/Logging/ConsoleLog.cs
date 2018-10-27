using System;

namespace DirSync.Logging
{
    public class ConsoleLog : ILog
    {
        public void AddInfo(string filename)
        {
            LogMessage($"Added \"{filename}\"", ConsoleColor.Green);
        }

        public void DeleteInfo(string filename)
        {
            LogMessage($"Deleted \"{filename}\"", ConsoleColor.Red);
        }

        public void ReplaceInfo(string filename)
        {
            LogMessage($"Replaced \"{filename}\"", ConsoleColor.Cyan);
        }

        public void Info(string message, ConsoleColor color = ConsoleColor.White)
        {
            LogMessage(message, color);
        }

        public void Warn(string message)
        {
            LogMessage(message, ConsoleColor.Yellow);
        }

        public void Error(string message)
        {
            LogMessage(message, ConsoleColor.DarkRed);
        }

        private static void LogMessage(string message, ConsoleColor color)
        {
            var consoleColor = Console.ForegroundColor;

            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = consoleColor;
        }
    }
}
