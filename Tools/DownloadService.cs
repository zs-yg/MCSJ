using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

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

        public async Task DownloadVersion(string version)
        {
            var url = _versionManager.GetDownloadUrl(version);
            if (url == null)
            {
                Console.WriteLine($"版本 {version} 不存在");
                return;
            }

            try
            {
                Console.WriteLine($"开始下载 {version}...");
                
                var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength ?? -1;
                var downloadedBytes = 0L;
                var buffer = new byte[8192];
                var isMoreToRead = true;

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream($"{version}.jar", FileMode.Create, FileAccess.Write))
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

                Console.WriteLine($"\n{version} 下载完成!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"下载失败: {ex.Message}");
            }
        }
    }
}
