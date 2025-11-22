using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO.Compression;
using MCSJ.Tools.LogSystem;

namespace MCSJ.Tools.JreDownload
{
    public class JreDownloadService
    {
        private readonly HttpClient _httpClient;
        private const string JreListPath = "resources/jrelist.txt";
        private const string JreRootFolder = "jre";
        private const string SetupFolder = "setup";

        public JreDownloadService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task DownloadAndSetupJre(string version)
        {
            // 0. 检查是否已存在该版本
            if (CheckJreExists(version))
            {
                Console.WriteLine($"JRE {version} 已存在，无需重复下载");
                LogMain.Info($"JRE {version} 已存在，无需重复下载");
                return;
            }

            // 1. 读取jrelist.txt获取下载链接
            var downloadUrl = GetDownloadUrl(version);
            if (string.IsNullOrEmpty(downloadUrl))
            {
                Console.WriteLine($"找不到版本 {version} 的下载链接");
                LogMain.Error($"找不到版本 {version} 的下载链接");
                return;
            }

            // 2. 下载压缩包
            var progress = new JreDownloadProgress();
            var tempZipPath = await DownloadJreZip(downloadUrl, version, progress);
            if (string.IsNullOrEmpty(tempZipPath))
            {
                Console.WriteLine("下载失败");
                LogMain.Error("下载失败");
                return;
            }

            // 3. 解压到jre文件夹
            var jreFolder = Path.Combine(JreRootFolder, version);
            if (!ExtractJre(tempZipPath, jreFolder))
            {
                Console.WriteLine("解压失败");
                LogMain.Error("解压失败");
                return;
            }

            // 4. 查找java.exe和javaw.exe
            var javaExePath = FindJavaExe(jreFolder, "java.exe");
            var javawExePath = FindJavaExe(jreFolder, "javaw.exe");

            if (javaExePath == null || javawExePath == null)
            {
                Console.WriteLine("找不到java.exe或javaw.exe");
                LogMain.Error("找不到java.exe或javaw.exe");
                return;
            }

            // 5. 生成jre.toml
            CreateJreToml(version, javaExePath, javawExePath);

            // 6. 清理临时文件
            File.Delete(tempZipPath);

                Console.WriteLine($"JRE {version} 安装完成");
                LogMain.Info($"JRE {version} 安装完成");
        }

        private string? GetDownloadUrl(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
                return null;

            if (!File.Exists(JreListPath))
                return null;

            var lines = File.ReadAllLines(JreListPath);
            return lines.FirstOrDefault(l => l.StartsWith(version + ":"))?.Split(':').LastOrDefault();
        }

        private async Task<string?> DownloadJreZip(string url, string version, IProgress<(long, long, int)>? progress = null)
        {
            var tempPath = Path.GetTempFileName();
            try
            {
                Console.WriteLine($"开始下载 JRE {version}...");
                // 确保URL是绝对路径，自动补全https协议头
                if (!url.StartsWith("http:") && !url.StartsWith("https:"))
                {
                    url = "https:" + url;
                }

                if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    Console.WriteLine($"无效的下载URL: {url}");
                    LogMain.Error($"无效的下载URL: {url}");
                    return null;
                }

                Console.WriteLine($"正在准备下载 {url}...");
                LogMain.Info($"正在准备下载 {url}...");
                var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength ?? 0;
                Console.WriteLine($"文件总大小: {totalBytes} 字节");
                LogMain.Info($"文件总大小: {totalBytes} 字节");
                long bytesRead = 0;
                var lastReportTime = DateTime.MinValue;
                Console.WriteLine("开始下载...");
                LogMain.Info("开始下载...");

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(tempPath, FileMode.Create))
                {
                    var buffer = new byte[8192];
                    int bytesReadThisPass;
                    
                    while ((bytesReadThisPass = await stream.ReadAsync(buffer)) != 0)
                    {
                        await fileStream.WriteAsync(buffer.AsMemory(0, bytesReadThisPass));
                        bytesRead += bytesReadThisPass;
                        
                        // 限制进度报告频率，避免过多控制台输出
                        if (DateTime.Now - lastReportTime > TimeSpan.FromMilliseconds(100) || bytesRead == totalBytes)
                        {
                            int progressPercentage = totalBytes > 0 ? (int)(bytesRead * 100 / totalBytes) : 0;
                            Console.WriteLine($"报告进度: {bytesRead}/{totalBytes} ({progressPercentage}%)");
                            progress?.Report((bytesRead, totalBytes, progressPercentage));
                            lastReportTime = DateTime.Now;
                        }
                    }
                }

                return tempPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"下载失败: {ex.Message}");
                LogMain.Error($"下载失败: {ex.Message}");
                return null;
            }
        }

        private bool ExtractJre(string zipPath, string targetFolder)
        {
            try
            {
                if (Directory.Exists(targetFolder))
                    Directory.Delete(targetFolder, true);

                Directory.CreateDirectory(targetFolder);
                ZipFile.ExtractToDirectory(zipPath, targetFolder);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"解压失败: {ex.Message}");
                LogMain.Error($"解压失败: {ex.Message}");
                return false;
            }
        }

        private string? FindJavaExe(string folder, string exeName)
        {
            if (string.IsNullOrWhiteSpace(folder) || string.IsNullOrWhiteSpace(exeName))
                return null;

            if (!Directory.Exists(folder))
                return null;

            return Directory.GetFiles(folder, exeName, SearchOption.AllDirectories).FirstOrDefault();
        }

        private bool CheckJreExists(string version)
        {
            var tomlPath = Path.Combine(SetupFolder, "jre.toml");
            if (!File.Exists(tomlPath))
                return false;

            try
            {
                var content = File.ReadAllText(tomlPath);
                return content.Contains($"[jre.{version}]");
            }
            catch
            {
                return false;
            }
        }

        private void CreateJreToml(string version, string javaExePath, string javawExePath)
        {
            if (!Directory.Exists(SetupFolder))
                Directory.CreateDirectory(SetupFolder);

            var tomlPath = Path.Combine(SetupFolder, "jre.toml");
            var content = $@"[jre.{version}]
java_path = '{javaExePath}'
javaw_path = '{javawExePath}'
";

            if (File.Exists(tomlPath))
            {
                var existingContent = File.ReadAllText(tomlPath);
                if (!existingContent.Contains($"[jre.{version}]"))
                {
                    File.AppendAllText(tomlPath, content);
                }
            }
            else
            {
                File.WriteAllText(tomlPath, content);
            }
        }
    }
}
