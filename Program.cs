using System;
using System.Threading.Tasks;
using MCSJ.Tools;

namespace MCSJ
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var versionManager = new VersionManager();
            var downloadService = new DownloadService(versionManager);
            
            while (true)
            {
                Console.WriteLine("MC服务器下载工具");
                Console.WriteLine("1. 显示所有版本");
                Console.WriteLine("2. 下载指定版本");
                Console.WriteLine("3. 退出");
                Console.Write("请选择操作: ");
                
                var input = Console.ReadLine();
                
                switch (input)
                {
                    case "1":
                        versionManager.DisplayAllVersions();
                        break;
                    case "2":
                        Console.Write("请输入要下载的版本名称: ");
                        var version = Console.ReadLine();
                        await downloadService.DownloadVersion(version);
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("无效输入，请重新选择");
                        break;
                }
                
                Console.WriteLine();
            }
        }
    }
}
