using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using MCSJ.Tools.LogSystem;

namespace MCSJ.Tools.ServerManagement
{
    public static class ServerStarter
    {
        public static void StartServer()
        {
            LogMain.Info("开始启动服务器流程");
            
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
            string serverPath = Path.Combine("profiles", selectedServer);
            string batPath = Path.Combine(serverPath, "start.bat");

            // 检查启动脚本是否存在
            if (!File.Exists(batPath))
            {
                LogMain.Warn($"没有找到启动脚本: {batPath}");
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
                    LogMain.Error($"启动脚本不存在: {fullBatPath}");
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

                LogMain.Info($"启动路径: {fullBatPath}");
                LogMain.Info($"工作目录: {fullServerPath}");
                Console.WriteLine($"启动路径: {fullBatPath}");
                Console.WriteLine($"工作目录: {fullServerPath}");

                Process.Start(startInfo);
                LogMain.Info($"成功启动服务器: {selectedServer}");
                Console.WriteLine($"已启动服务器: {selectedServer}");
            }
            catch (Exception ex)
            {
                LogMain.Error($"启动服务器失败: {ex.Message}");
                Console.WriteLine($"启动服务器失败: {ex.Message}");
            }
        }
    }
}
