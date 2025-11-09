using System;
using System.IO;

namespace MCSJ.Tools.LogSystem
{
    public static class LogCreator
    {
        public static string GenerateLogFileName()
        {
            DateTime now = DateTime.Now;
            return $"{now:yyyy-MM-dd-HH-mm-ss}.log";
        }
        
        public static string GetLogDirectory()
        {
            string logDir = Path.Combine(Directory.GetCurrentDirectory(), "logs");
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            return logDir;
        }
        
        public static string GetLogFilePath()
        {
            return Path.Combine(GetLogDirectory(), GenerateLogFileName());
        }
    }
}
