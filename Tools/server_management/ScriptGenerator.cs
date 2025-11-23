using System;
using System.IO;
using System.Collections.Generic;
using MCSJ.Tools.LogSystem;

namespace MCSJ.Tools.ServerManagement
{
    public static class ScriptGenerator
    {
        public static void GenerateScript()
        {
            LogMain.Info("开始生成服务器启动脚本");
            
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
                LogMain.Error($"无效的服务器选择: {serverIndex}");
                Console.WriteLine("无效选择");
                return;
            }

            string selectedServer = servers[serverIndex - 1];
            LogMain.Info($"用户选择了服务器: {selectedServer}");
            string serverPath = Path.Combine("profiles", selectedServer);

            // 检查JRE配置
            string jreConfigPath = "setup/jre.toml";
            if (!File.Exists(jreConfigPath) || new FileInfo(jreConfigPath).Length == 0)
            {
                LogMain.Error("没有找到有效的JRE配置");
                Console.WriteLine("没有下载的JRE，请先下载JRE");
                return;
            }

            // 解析JRE配置
            var jreVersions = new Dictionary<string, Dictionary<string, string>>();
            string currentVersion = null;
            
            foreach (var line in File.ReadAllLines(jreConfigPath))
            {
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    currentVersion = line.Trim('[', ']');
                    jreVersions[currentVersion] = new Dictionary<string, string>();
                }
                else if (line.Contains("=") && currentVersion != null)
                {
                    var parts = line.Split('=');
                    string key = parts[0].Trim();
                    string value = parts[1].Trim().Trim('\'');
                    jreVersions[currentVersion][key] = value;
                }
            }

            if (jreVersions.Count == 0)
            {
                LogMain.Error("JRE配置文件为空");
                Console.WriteLine("没有可用的JRE配置");
                return;
            }

            // 选择JRE版本
            Console.WriteLine("可用的JRE版本:");
            int versionIndex = 1;
            var versionList = new List<string>(jreVersions.Keys);
            foreach (var version in versionList)
            {
                Console.WriteLine($"{versionIndex}. {version}");
                versionIndex++;
            }

            Console.Write("请选择JRE版本(输入编号): ");
            if (!int.TryParse(Console.ReadLine(), out int selectedVersionIndex) || 
                selectedVersionIndex < 1 || selectedVersionIndex > versionList.Count)
            {
                Console.WriteLine("无效选择");
                return;
            }

            string selectedVersion = versionList[selectedVersionIndex - 1];
            LogMain.Info($"用户选择了JRE版本: {selectedVersion}");
            var versionConfig = jreVersions[selectedVersion];

            if (!versionConfig.ContainsKey("java_path") || !versionConfig.ContainsKey("javaw_path"))
            {
                LogMain.Error($"JRE配置不完整: {selectedVersion}");
                Console.WriteLine("JRE配置不完整");
                return;
            }

            // 选择java执行方式
            Console.WriteLine("选择Java执行方式:");
            Console.WriteLine("1. java.exe (控制台模式)");
            Console.WriteLine("2. javaw.exe (无控制台模式)");
            Console.Write("请选择: ");
            string javaPath = Console.ReadLine() == "1" 
                ? Path.GetFullPath(versionConfig["java_path"])
                : Path.GetFullPath(versionConfig["javaw_path"]);

            // 生成启动脚本(统一使用反斜杠)
            string scriptContent = $"@echo off\r\n" +
                                 $"cd /D \"{Path.GetFullPath(serverPath).Replace('/', '\\')}\"\r\n" +
                                 $"\"{javaPath.Replace('/', '\\')}\" -jar server.jar\r\n" +
                                 "pause";

            string scriptPath = Path.Combine(serverPath, "start.bat");
            File.WriteAllText(scriptPath, scriptContent);
            LogMain.Info($"成功生成启动脚本: {scriptPath}");
            Console.WriteLine($"已生成启动脚本: {scriptPath}");
        }
    }
}
