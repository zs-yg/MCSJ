using System;
using System.IO;
using System.Linq;
using MCSJ.Tools.LogSystem;

namespace MCSJ.Tools.ViewJre
{
    public class JreViewer
    {
        private const string SetupFolder = "setup";
        private const string JreTomlFile = "jre.toml";

        public void DisplayInstalledJres()
        {
            var tomlPath = Path.Combine(SetupFolder, JreTomlFile);
            
            if (!File.Exists(tomlPath))
            {
                Console.WriteLine("没有安装任何JRE");
                LogMain.Info("没有安装任何JRE");
                return;
            }

            try
            {
                var content = File.ReadAllText(tomlPath);
                var versions = content.Split('\n')
                    .Where(line => line.StartsWith("[jre."))
                    .Select(line => line.Split('.')[1].Split(']')[0].Trim()) // 精确提取版本号
                    .ToList();

                if (versions.Count == 0)
                {
                Console.WriteLine("没有安装任何JRE");
                LogMain.Info("没有安装任何JRE");
                return;
                }

                Console.WriteLine("已安装的JRE版本:");
                LogMain.Info("已安装的JRE版本:");
                foreach (var version in versions)
                {
                    Console.WriteLine(version); // 直接输出版本号，不带前缀
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取JRE列表失败: {ex.Message}");
                LogMain.Error($"读取JRE列表失败: {ex.Message}");
            }
        }
    }
}
