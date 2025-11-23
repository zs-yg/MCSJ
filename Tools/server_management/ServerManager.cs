using System;
using System.IO;
using System.Collections.Generic;

namespace MCSJ.Tools.ServerManagement
{
    public class ServerManager
    {
        private const int SERVER_MANAGEMENT_OPTION = 3;
        
        public static void ShowMenu()
        {
            while (true)
            {
                Console.WriteLine("\n=== 服务器管理 ===");
                Console.WriteLine("1. 生成启动脚本");
                Console.WriteLine("2. 启动服务器");
                Console.WriteLine("3. 同意eula.txt");
                Console.WriteLine("4. 返回主菜单");
                Console.Write("请选择: ");
                
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ScriptGenerator.GenerateScript();
                        break;
                    case "2":
                        ServerStarter.StartServer();
                        break;
                    case "3":
                        EulaAgreer.AgreeEula();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("无效选项");
                        break;
                }
                
                Console.WriteLine("\n按任意键继续...");
                Console.ReadKey();
            }
        }
        
        public static List<string> GetServerProfiles()
        {
            var profiles = new List<string>();
            if (Directory.Exists("profiles"))
            {
                foreach (var dir in Directory.GetDirectories("profiles"))
                {
                    profiles.Add(Path.GetFileName(dir));
                }
            }
            return profiles;
        }
    }
}
