using System;
using System.IO;
using System.Collections.Generic;
using MCSJ.Tools.LogSystem;

namespace MCSJ.Tools.ServerManagement
{
    public static class EulaAgreer
    {
        public static void AgreeEula()
        {
            LogMain.Info("开始处理EULA同意流程");
            
            // 获取服务器列表
            var servers = ServerManager.GetServerProfiles();
            if (servers.Count == 0)
            {
                LogMain.Error("没有可用的服务器存档");
                Console.WriteLine("没有可用的服务器存档");
                return;
            }

            // 显示服务器列表供选择
            LogMain.Info($"找到 {servers.Count} 个服务器存档");
            Console.WriteLine("可用的服务器存档:");
            for (int i = 0; i < servers.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {servers[i]}");
            }

            Console.Write("请选择服务器(输入编号): ");
            if (!int.TryParse(Console.ReadLine(), out int serverIndex) || serverIndex < 1 || serverIndex > servers.Count)
            {
                LogMain.Warn($"无效的服务器选择: {serverIndex}");
                Console.WriteLine("无效选择");
                return;
            }

            string selectedServer = servers[serverIndex - 1];
            LogMain.Info($"用户选择了服务器: {selectedServer}");
            string eulaPath = Path.Combine("profiles", selectedServer, "eula.txt");

            // 检查eula文件是否存在
            if (!File.Exists(eulaPath))
            {
                LogMain.Warn($"没有找到eula.txt文件: {eulaPath}");
                Console.WriteLine("没有找到eula.txt文件");
                return;
            }

            // 读取并修改eula文件
            string eulaContent = File.ReadAllText(eulaPath);
            if (eulaContent.Contains("eula=true"))
            {
                LogMain.Info("EULA已经同意，无需修改");
                Console.WriteLine("EULA已经同意，无需修改");
                return;
            }

            if (eulaContent.Contains("eula=false"))
            {
                eulaContent = eulaContent.Replace("eula=false", "eula=true");
                File.WriteAllText(eulaPath, eulaContent);
                LogMain.Info("已成功同意EULA");
                Console.WriteLine("已同意EULA");
            }
            else
            {
                LogMain.Error($"无效的eula.txt格式: {eulaPath}");
                Console.WriteLine("无效的eula.txt格式");
            }
        }
    }
}
