using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace MCSJ.Tools.ServerManagement
{
    public static class ServerStarter
    {
        public static void StartServer()
        {
            // 获取服务器列表
            var servers = ServerManager.GetServerProfiles();
            if (servers.Count == 0)
            {
                Console.WriteLine("没有可用的服务器存档");
                return;
            }

            // 显示服务器列表供选择
            Console.WriteLine("可用的服务器存档:");
            for (int i = 0; i < servers.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {servers[i]}");
            }

            Console.Write("请选择服务器(输入编号): ");
            if (!int.TryParse(Console.ReadLine(), out int serverIndex) || serverIndex < 1 || serverIndex > servers.Count)
            {
                Console.WriteLine("无效选择");
                return;
            }

            string selectedServer = servers[serverIndex - 1];
            string serverPath = Path.Combine("profiles", selectedServer);
            string batPath = Path.Combine(serverPath, "start.bat");

            // 检查启动脚本是否存在
            if (!File.Exists(batPath))
            {
                Console.WriteLine("没有找到启动脚本，请先生成脚本");
                return;
            }

            // 启动服务器
            try
            {
                string fullBatPath = Path.GetFullPath(batPath);
                string fullServerPath = Path.GetFullPath(serverPath);

                if (!File.Exists(fullBatPath))
                {
                    Console.WriteLine($"启动脚本不存在: {fullBatPath}");
                    return;
                }

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = fullBatPath,
                    WorkingDirectory = fullServerPath,
                    UseShellExecute = true,
                    CreateNoWindow = false
                };

                Console.WriteLine($"启动路径: {fullBatPath}");
                Console.WriteLine($"工作目录: {fullServerPath}");

                Process.Start(startInfo);
                Console.WriteLine($"已启动服务器: {selectedServer}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"启动服务器失败: {ex.Message}");
            }
        }
    }
}
