using System;
using System.Threading.Tasks;
using MCSJ.Tools;
using MCSJ.Tools.LogSystem;
using MCSJ.Tools.JreDownload;
using MCSJ.Tools.ViewJre;

namespace MCSJ
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // 验证日志目录
            string logDir = LogCreator.GetLogDirectory();
            Console.WriteLine($"日志目录: {logDir}");
            LogMain.Debug($"日志文件: {LogCreator.GetLogFilePath()}");
            
            LogMain.Info("MC服务器下载工具启动");
            var httpClient = new HttpClient {
                Timeout = TimeSpan.FromMinutes(5),
                DefaultRequestHeaders = { { "User-Agent", "MCSJ-JRE-Downloader" } }
            };
            var versionManager = new VersionManager();
            var downloadService = new DownloadService(versionManager);
            var jreViewer = new JreViewer();
            LogMain.Debug("服务初始化完成");
            
            while (true)
            {
                Console.WriteLine("MC服务器下载工具");
                Console.WriteLine("1. 显示所有版本");
                Console.WriteLine("2. 下载指定版本");
                Console.WriteLine("3. 下载JRE");
                Console.WriteLine("4. 查看已安装的JRE");
                Console.WriteLine("5. 退出");
                Console.Write("请选择操作: ");
                
                var input = Console.ReadLine();
                LogMain.Debug($"用户选择操作: {input}");
                
                switch (input)
                {
                    case "1":
                        versionManager.DisplayAllVersions();
                        LogMain.Info("显示所有版本列表");
                        break;
                    case "2":
                        Console.Write("请输入要下载的版本名称: ");
                        var version = Console.ReadLine();
                        LogMain.Info($"开始下载版本: {version}");
                        await downloadService.DownloadVersion(version);
                        LogMain.Info($"版本下载完成: {version}");
                        break;
                    case "3":
                        Console.Write("请输入要下载的JRE版本(jre8,jre11,jre17/21/25): ");
                        var jreVersion = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(jreVersion))
                        {
                            Console.WriteLine("JRE版本不能为空");
                            continue;
                        }
                        LogMain.Info($"开始下载JRE: {jreVersion}");
                        var jreDownloadService = new JreDownloadService(httpClient);
                        await jreDownloadService.DownloadAndSetupJre(jreVersion);
                        LogMain.Info($"JRE下载完成: {jreVersion}");
                        break;
                    case "4":
                        jreViewer.DisplayInstalledJres();
                        LogMain.Info("显示已安装的JRE列表");
                        break;
                    case "5":
                        LogMain.Info("程序正常退出");
                        return;
                    default:
                        Console.WriteLine("无效输入，请重新选择");
                        LogMain.Warn($"无效的用户输入: {input}");
                        break;
                }
                
                Console.WriteLine();
            }
        }
    }
}
