using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using MCSJ.Tools.LogSystem;

namespace MCSJ.Tools
{
    public class DownloadService
    {
        private readonly VersionManager _versionManager;
        private readonly HttpClient _httpClient;

        public DownloadService(VersionManager versionManager)
        {
            _versionManager = versionManager;
            _httpClient = new HttpClient();
        }

        public async Task DownloadVersion(string? version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                Console.WriteLine("版本名称不能为空");
                LogMain.Error("版本名称不能为空");
                return;
            }

            var url = _versionManager.GetDownloadUrl(version);
            if (string.IsNullOrEmpty(url))
            {
                Console.WriteLine($"版本 {version} 不存在");
                LogMain.Error($"版本 {version} 不存在");
                return;
            }

            // 根目录 profiles 文件夹
            var profilesRoot = Path.Combine(Directory.GetCurrentDirectory(), "profiles");
            if (!Directory.Exists(profilesRoot))
                Directory.CreateDirectory(profilesRoot);

            string? targetFolder = null;
            string? profilePath = null;
            while (true)
            {
                Console.Write($"请输入存放文件夹名称（直接回车默认用版本名 '{version}'）：");
                var input = Console.ReadLine();
                targetFolder = string.IsNullOrWhiteSpace(input) ? version : input;
                profilePath = Path.Combine(profilesRoot, targetFolder);
                if (!Directory.Exists(profilePath))
                    break;
                Console.WriteLine($"文件夹 '{targetFolder}' 已存在，请重新输入（直接回车则取消下载）：");
                LogMain.Warn($"文件夹 '{targetFolder}' 已存在，请重新输入（直接回车则取消下载）：");
            }
            if (Directory.Exists(profilePath))
            {
                Console.WriteLine("下载已取消。");
                LogMain.Info("下载已取消。");
                return;
            }
            Directory.CreateDirectory(profilePath);
            string jarPath = Path.Combine(profilePath, "server.jar");

            try
            {
                Console.WriteLine($"开始下载 {version} 到 {profilePath} ...");
                LogMain.Info($"开始下载 {version} 到 {profilePath} ...");
                var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength ?? -1;
                var downloadedBytes = 0L;
                var buffer = new byte[8192];
                var isMoreToRead = true;

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(jarPath, FileMode.Create, FileAccess.Write))
                {
                    while (isMoreToRead)
                    {
                        var read = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (read == 0)
                        {
                            isMoreToRead = false;
                        }
                        else
                        {
                            await fileStream.WriteAsync(buffer, 0, read);
                            downloadedBytes += read;
                            if (totalBytes > 0)
                            {
                                var progress = (double)downloadedBytes / totalBytes * 100;
                                Console.Write($"\r下载进度: {progress:F2}%");
                            }
                        }
                    }
                }
                Console.WriteLine($"\n{version} 下载完成! 文件已保存到 {jarPath}");
                LogMain.Info($"{version} 下载完成! 文件已保存到 {jarPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"下载失败: {ex.Message}");
                LogMain.Error($"下载失败: {ex.Message}");
            }
        }
    }
}
