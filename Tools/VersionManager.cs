using System;
using System.Collections.Generic;
using System.IO;

namespace MCSJ.Tools
{
    public class VersionManager
    {
        private readonly Dictionary<string, string> _versions = new();

        public VersionManager()
        {
            LoadVersions();
        }

        private void LoadVersions()
        {
            try
            {
                var filePath = Path.Combine("resources", "serverlist.txt");
                Console.WriteLine($"尝试从路径加载版本列表: {Path.GetFullPath(filePath)}");
                
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"服务器列表文件不存在: {filePath}");
                }

                var lines = File.ReadAllLines(filePath);

                foreach (var rawLine in lines)
                {
                    var line = rawLine.Trim();
                    // 跳过空行和注释
                    if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                        continue;

                    var colonIndex = line.IndexOf(':');
                    if (colonIndex > 0 && colonIndex < line.Length - 1)
                    {
                        var version = line.Substring(0, colonIndex).Trim();
                        var url = line.Substring(colonIndex + 1).Trim();

                        if (string.IsNullOrEmpty(version) || string.IsNullOrEmpty(url))
                        {
                            Console.WriteLine($"忽略无效条目: {rawLine} (版本或URL为空)");
                            continue;
                        }

                        _versions[version] = url;
                    }
                    else
                    {
                        Console.WriteLine($"忽略无效条目: {rawLine} (缺少冒号分隔或格式不正确)");
                    }
                }
                
                if (_versions.Count == 0)
                {
                    throw new Exception("没有找到有效的版本条目");
                }
                
                Console.WriteLine($"成功加载 {_versions.Count} 个版本");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载版本列表失败: {ex.Message}");
                Console.WriteLine($"当前工作目录: {Directory.GetCurrentDirectory()}");
                Console.WriteLine("请确保serverlist.txt每行格式为: 版本名:下载URL (版本名可以包含空格)，支持以#开头的注释");
            }
        }

        public void DisplayAllVersions()
        {
            var filePath = Path.Combine("resources", "serverlist.txt");
            if (!File.Exists(filePath))
            {
                Console.WriteLine("版本列表文件不存在");
                return;
            }

            Console.WriteLine("可用版本列表:");
            foreach (var version in _versions.Keys)
            {
                Console.WriteLine(version);
            }
        }

        public string? GetDownloadUrl(string version)
        {
            return _versions.TryGetValue(version, out var url) ? url : null;
        }
    }
}
