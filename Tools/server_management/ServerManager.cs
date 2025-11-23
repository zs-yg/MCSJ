using System;
using System.IO;
using System.Collections.Generic;
using MCSJ.Tools.LogSystem;

namespace MCSJ.Tools.ServerManagement
{
    public class ServerManager
    {
        private const int SERVER_MANAGEMENT_OPTION = 3;
        
        public static void ShowMenu()
        {
            while (true)
            {
                LogMain.Info("显示服务器管理菜单");
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
                        LogMain.Info("用户选择: 生成启动脚本");
                        ScriptGenerator.GenerateScript();
                        break;
                    case "2":
                        LogMain.Info("用户选择: 启动服务器");
                        ServerStarter.StartServer();
                        break;
                    case "3":
                        LogMain.Info("用户选择: 同意eula.txt");
                        EulaAgreer.AgreeEula();
                        break;
                    case "4":
                        LogMain.Info("用户选择: 返回主菜单");
                        return;
                    default:
                        LogMain.Warn($"无效菜单选项: {choice}");
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
                LogMain.Info("获取服务器存档列表");
                foreach (var dir in Directory.GetDirectories("profiles"))
                {
                    profiles.Add(Path.GetFileName(dir));
                }
            }
            else
            {
                LogMain.Warn("服务器存档目录不存在");
            }
            return profiles;
        }
    }
}
