using System;

namespace DirSync.Logging
{
    public interface ILog
    {
        void AddInfo(string filename);
        void DeleteInfo(string filename);
        void ReplaceInfo(string filename);

        void Info(string message, ConsoleColor color = ConsoleColor.White);
        void Warn(string message);
        void Error(string message);
    }
}
