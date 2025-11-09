using System;
using System.IO;
using System.Threading;

namespace MCSJ.Tools.LogSystem
{
    public static class LogMain
    {
        private static readonly object _lock = new object();
        
        public enum LogLevel
        {
            DEBUG,
            INFO,
            WARN,
            ERROR,
            FATAL
        }
        
        public static void Log(LogLevel level, string message)
        {
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
            string logPath = LogCreator.GetLogFilePath();
            
            lock (_lock)
            {
                File.AppendAllText(logPath, logEntry + Environment.NewLine);
            }
        }
        
        public static void Debug(string message) => Log(LogLevel.DEBUG, message);
        public static void Info(string message) => Log(LogLevel.INFO, message);
        public static void Warn(string message) => Log(LogLevel.WARN, message);
        public static void Error(string message) => Log(LogLevel.ERROR, message);
        public static void Fatal(string message) => Log(LogLevel.FATAL, message);
    }
}
