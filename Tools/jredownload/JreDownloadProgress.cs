using System;
using System.IO;

namespace MCSJ.Tools.JreDownload
{
    public class JreDownloadProgress : IProgress<(long bytesReceived, long totalBytesToReceive, int progressPercentage)>
    {
        public void Report((long bytesReceived, long totalBytesToReceive, int progressPercentage) value)
        {
            // 保存当前控制台颜色
            var originalColor = Console.ForegroundColor;
            
            try
            {
                // 计算下载百分比
                int percentage = value.progressPercentage;
                
                // 格式化文件大小（使用MB单位）
                string received = FormatFileSizeMB(value.bytesReceived);
                string total = FormatFileSizeMB(value.totalBytesToReceive);
                
                // 创建进度条
                string progressBar = CreateProgressBar(percentage);
                
                // 显示进度信息（仅进度条部分为绿色）
                Console.Write("\r下载进度: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{progressBar}");
                Console.ForegroundColor = originalColor;
                Console.Write($" {percentage}% [{FormatFileSizeMB(value.bytesReceived)} / {FormatFileSizeMB(value.totalBytesToReceive)}]");
                
                // 下载完成时换行
                if (percentage == 100)
                {
                    Console.WriteLine();
                }
            }
            finally
            {
                // 恢复原始控制台颜色
                Console.ForegroundColor = originalColor;
            }
        }
        
        private string CreateProgressBar(int percentage)
        {
            int width = 20; // 进度条宽度
            int progress = percentage * width / 100;
            
            return $"[{new string('#', progress)}{new string('-', width - progress)}]";
        }
        
        private string FormatFileSizeMB(long bytes)
        {
            double mb = bytes / (1024.0 * 1024.0);
            return $"{mb:0.##} MB";
        }
    }
}
