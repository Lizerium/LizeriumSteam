/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 02 мая 2026 19:36:41
 * Version: 1.0.39
 */

using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

using AppUpdater.Log;
using AppUpdater.Manifest;

namespace AppUpdater.Server
{
    public class DefaultUpdateServer : IUpdateServer
    {
        private readonly Uri updateServerUrl;
        private readonly ILog log = Logger.For<DefaultUpdateServer>();
        public EventHandler<DataDownloadHandle> UpdateDownloadHandle { get; set; }
        public EventHandler<string> VersionReceive {  get; set; }

        public DefaultUpdateServer(Uri updateServerUrl)
        {
            this.updateServerUrl = updateServerUrl;
        }

        public string GetLastVersionMode(string nameMode)
        {
            Uri versionUrl = new Uri(updateServerUrl, nameMode + "/version");

            // Создаем делегат для обратного вызова
            Action<string> callback = (response) =>
            {
                VersionReceive.Invoke(this, response);
            };

            // Создаем объект WebClient
            using (var client = new WebClient())
            {
                // Add certificate validation callback
                ServicePointManager.ServerCertificateValidationCallback += ValidateCertificate;
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

                // Используем BackgroundWorker для асинхронной загрузки
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (sender, e) =>
                {
                    try
                    {
                        log.Debug("Downloading from url: {0}", versionUrl);
                        e.Result = client.DownloadString(versionUrl); // Сохраняем результат в e.Result
                    }
                    catch (Exception ex)
                    {
                        log.Error("Error downloading from url: {0}", ex.Message);
                        e.Result = string.Empty;
                    }
                };

                // Обработка завершения загрузки
                worker.RunWorkerCompleted += (sender, e) =>
                {
                    if (e.Error != null)
                    {
                        // Обработка ошибки
                        log.Error("Error during download: {0}", e.Error.Message);
                    }
                    else
                    {
                        string response = (string)e.Result; // Получение результата
                                                            // Вызываем обратный вызов
                        callback.Invoke(response);
                    }
                };

                // Запуск асинхронной задачи
                worker.RunWorkerAsync();

                return string.Empty; // Возвращаем пустую строку, пока загрузка не завершится
            }
        }

        public string GetCurrentUpdateMode(string nameMode)
        {
            Uri updatesUrl = new Uri(updateServerUrl, nameMode + "/updates");
            var client = new WebClient();
            // Add certificate validation callback
            ServicePointManager.ServerCertificateValidationCallback += ValidateCertificate;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            try
            {
                log.Debug("Downloading from url: {0}", updatesUrl);
                string response = client.DownloadString(updatesUrl);

                if (string.IsNullOrEmpty(response))
                {
                    log.Error("Информация не получена...");
                    return string.Empty;
                }

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response);
                return doc.SelectSingleNode("config/updates").InnerText;
            }
            catch (Exception ex)
            {
                log.Error("Error downloading from url: {0}", ex.Message);
                return string.Empty;
            }
        }

        public async Task<string> GetCurrentUpdateModeAsync(string nameMode)
        {
            Uri updatesUrl = new Uri(updateServerUrl, nameMode + "/updates");

           var client = new HttpClient(
                new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback 
                    = (sender, cert, chain, sslPolicyErrors) => true
                });

            try
            {
                log.Debug("Downloading from url: {0}", updatesUrl);

                var response = await client.GetStringAsync(updatesUrl);

                if (string.IsNullOrEmpty(response))
                {
                    log.Error("Информация не получена...");
                    return string.Empty;
                }

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response);
                return doc.SelectSingleNode("config/updates").InnerText;
            }
            catch (Exception ex)
            {
                log.Error("Error downloading from url: {0}", ex.Message);
                return string.Empty;
            }
        }


        public async Task<string> GetCurrentVersion()
        {
            string xmlData = await DownloadStringBack("version.xml");

            if(string.IsNullOrEmpty(xmlData))
            {
                log.Error("Информация не получена...");
                return string.Empty;
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlData);
            return doc.SelectSingleNode("config/version").InnerText;
        }

        public async Task<VersionManifest> GetManifest(string version)
        {
            string xmlData = await DownloadStringBack(GetVersionFilename(version, "manifest.xml"));
            return VersionManifest.LoadVersionData(version, xmlData);
        }

        public async Task<VersionManifest> GetModeManifest(string ModeName, string version)
        {
            string xmlData = await DownloadStringBack(GetModeVersionFilename("mod", version, "manifest.xml", ModeName));
            if (string.IsNullOrEmpty(xmlData)) return null;
            return VersionManifest.LoadVersionData(version, xmlData);
        }

        public async Task<byte[]> DownloadFile(CancellationToken token, string version, string filename)
        {
            return await DownloadBinary(GetVersionFilename(version, filename), token);
        }

        public async Task<byte[]> DownloadFile(CancellationToken token, string nameMode, string version, string filename)
        {
            return await DownloadBinary(GetVersionFilename(nameMode, version, filename), token);
        }

        public async Task<byte[]> DownloadUpdateFile(CancellationToken token, string nameMode, string version, string directory)
        {
            var path = Path.Combine(Path.GetTempPath(), nameMode, "updates");
            var url = GetUpdateModeFilename(nameMode, version);
            return await DownloadBinaryHttpClientAsync(token, url, path, version + ".tar", 360000);
        }

        public async Task<byte[]> DownloadAndSaveFile(CancellationToken token, string path, string nameMode, string version, string filename)
        {
            return await DownloadBinary(GetVersionFilename(nameMode, version, filename), token, true, path, filename);
        }

        public string GetURL(string path, string nameMode, string version, string filename)
        {
            return GetVersionFilename(nameMode, version, filename);
        }

        private string DownloadString(string filename)
        {
            Uri versionUrl = new Uri(updateServerUrl, filename);
            var client = new WebClient();
            // Add certificate validation callback
            ServicePointManager.ServerCertificateValidationCallback += ValidateCertificate;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            try
            {
                log.Debug("Downloading from url: {0}", versionUrl);
                string response = client.DownloadString(versionUrl);
                return response;
            }
            catch (Exception ex)
            {
                log.Error("Error downloading from url: {0}", ex.Message);
                return string.Empty;
            }
        }

        private async Task<bool> IsServerReachableAsync(string url)
        {
            try
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = ValidateCertificate;
                using (var client = new HttpClient(handler))
                {
                    var pingUrl = new Uri(url.TrimEnd('/') + "/ping");
                    // Отправляем GET-запрос для проверки доступности
                    var response = await client.GetAsync(pingUrl);
                    // Проверяем код ответа
                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                log.Error("Error checking server reachability: {0}", ex.Message);
                return false;
            }
        }

        private async Task<string> DownloadStringBack(string filename)
        {
            try
            {
                bool enableSrv = await IsServerReachableAsync(updateServerUrl.OriginalString);
                if (enableSrv)
                {
                    Uri versionUrl = new Uri(updateServerUrl, filename);
                    var client = new WebClient();
                    // Add certificate validation callback
                    ServicePointManager.ServerCertificateValidationCallback += ValidateCertificate;
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    try
                    {
                        log.Debug("Downloading from url: {0}", versionUrl);
                        string response = client.DownloadString(versionUrl);
                        return response;
                    }
                    catch (Exception ex)
                    {
                        log.Error("Error downloading from url: {0}", ex.Message);
                        return string.Empty;
                    }
                }
                else
                {
                    // Сервер недоступен, выводим сообщение или обрабатываем ошибку
                    log.Error("Server is not reachable.");
                    return string.Empty; // Возвращаем пустую строку, чтобы показать, что ничего не загружено
                }
            }
            catch (Exception ex)
            {
                log.Error("Error pinging server: {0}", ex.Message);
                return string.Empty; // Возвращаем пустую строку в случае ошибки
            }
        }


        // Certificate validation callback method
        public static bool ValidateCertificate(object sender, 
            X509Certificate certificate, 
            X509Chain chain, 
            System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            // Always return true to bypass certificate validation
            return true;
        }

        private async Task<byte[]> DownloadBinary(string URLFilename, CancellationToken token, bool mega = false, string path = "", string filename = "")
        {
            if (token.IsCancellationRequested) return null;
            if (mega)
                return await DownloadBinaryHttpClientAsync(token, URLFilename, path, filename, 360000);
            else return await DownloadBinaryWebClientAsync(token, URLFilename);
        }

        public async Task<byte[]> DownloadBinaryWebClientAsync(CancellationToken token, string fileName)
        {
            Uri versionUrl = new Uri(fileName);
            WebClient client = new WebClient();
            log.Debug("Downloading from url: {0}", versionUrl);
            return client.DownloadData(versionUrl);
        }

        public async Task<byte[]> DownloadBinaryWebAsync(Uri url)
        {
            var client = new WebClient();
            try
            {
                return client.DownloadData(url);
            }
            finally
            {
                client.Dispose();
            }
        }

        public async Task<byte[]> DownloadBinaryHttpAsync(CancellationToken token, string url)
        {
            var client = new HttpClient();
            log.Debug("Downloading from url: {0}", url);
            try
            {
                // Отправляем запрос, чтобы получить поток ответа
                HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var ms = new MemoryStream())
                {
                    var buffer = new byte[81920];
                    int bytesRead;

                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        token.ThrowIfCancellationRequested();

                        ms.Write(buffer, 0, bytesRead);
                    }

                    return ms.ToArray();
                }
            }
            finally
            {
                client.Dispose();
            }
        }

        public async Task<byte[]> DownloadBinaryHttpClientAsync(CancellationToken token, string URLFilename, string path, string fileName, int timeoutMilliseconds = 60000)
        {
            Uri versionUrl = new Uri(updateServerUrl, URLFilename);

            ServicePointManager.DefaultConnectionLimit = 20;
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(30);
                var filePath = Path.Combine(path, "updates", fileName);
                Stream contentStream = null;
                FileStream fileStream = null;

                //проверяю существование такого файла в папке и её существование
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                using (CancellationTokenSource timeoutCts = new CancellationTokenSource())
                // Create a cancellation token source
                using (CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, timeoutCts.Token))
                {
                    // Set a timer to cancel the download after the timeout
                    using (Timer timer = new Timer(
                        (state) => timeoutCts.Cancel(),
                        null,
                        timeoutMilliseconds,
                        Timeout.Infinite))
                    {
                        try
                        {
                            //log.Debug("Downloading from url: {0}", versionUrl);
                            // Send the request (GET)
                            HttpResponseMessage response = await client.GetAsync(versionUrl,
                                HttpCompletionOption.ResponseHeadersRead,
                                linkedCts.Token);

                            // Ensure the request was successful (status code 200)
                            response.EnsureSuccessStatusCode();

                            filePath = Path.Combine(path, fileName);
                            // Get the total file size
                            long totalFileSize = response.Content.Headers.ContentLength.GetValueOrDefault();

                            // Get the content stream
                            contentStream = await response.Content.ReadAsStreamAsync();

                            var rootRedist = Path.GetDirectoryName(filePath);
                            if (!Directory.Exists(rootRedist))
                                Directory.CreateDirectory(rootRedist);

                            // Create a FileStream to write the downloaded data
                            using (fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                            {
                                // Track downloaded bytes and start time
                                long totalBytesDownloaded = 0;
                                DateTime startTime = DateTime.Now;

                                // Read the stream in chunks
                                byte[] buffer = new byte[1024 * 1024 * 2]; // 2 MB

                                int bytesRead;

                                while (!linkedCts.Token.IsCancellationRequested && (bytesRead = await 
                                    contentStream.ReadAsync(buffer, 0, buffer.Length, linkedCts.Token)) > 0)
                                {
                                    // Update progress
                                    totalBytesDownloaded += bytesRead;

                                    // Calculate download speed
                                    TimeSpan elapsedTime = DateTime.Now - startTime;
                                    double downloadSpeed = (totalBytesDownloaded / (double)elapsedTime.TotalSeconds) / (1024 * 1024); // Bytes per second to KB/s

                                    //log.Debug($"Downloaded {totalBytesDownloaded} bytes. Speed: {downloadSpeed:F2} МБ/s");

                                    UpdateDownloadHandle.Invoke(this, new DataDownloadHandle()
                                    {
                                        DownloadSize = totalBytesDownloaded,
                                        DownloadSpeed = downloadSpeed,
                                        TotalFileSize = totalFileSize,
                                        NameFile = fileName
                                    });

                                    // Write the chunk to the file
                                    await fileStream.WriteAsync(buffer, 0, bytesRead, linkedCts.Token);
                                }
                                return null;
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            if (fileStream != null)
                            {
                                fileStream.Dispose();

                                // 🧹 Иногда помогает вручную подсказать GC
                                GC.Collect();
                                GC.WaitForPendingFinalizers();
                            }

                            if (timeoutCts.IsCancellationRequested)
                                log.Warn("Download timed out: {0}", versionUrl);
                            else if (token.IsCancellationRequested)
                            {
                                log.Warn("Download manually cancelled: {0}", versionUrl);
                                timeoutCts.Cancel();
                                linkedCts.Cancel();
                            }
                            try
                            {
                                // 💥 Гарантированное освобождение ресурсов
                                if (contentStream != null)
                                {
                                    try { contentStream.Dispose(); } catch { }
                                }

                                if (fileStream != null)
                                {
                                    try { fileStream.Dispose(); } catch { }
                                }

                                if (File.Exists(filePath))
                                {
                                    File.Delete(filePath); // 💣 удаляем "висевший" файл
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Warn("Could not delete partial file: " + ex.Message);
                            }
                            return null; // Or throw a custom exception
                        }
                        catch (HttpRequestException ex)
                        {
                            // Handle potential HTTP errors
                            log.Error("HTTP Error during download: {0}", ex.Message);
                            return null; // Or throw a custom exception
                        }
                        finally
                        {
                            // Dispose of the cancellation token source and timer
                            log.Debug("Download finished (clean-up complete): {0}", versionUrl);
                        }
                    }
                }
            }
        }

        private string GetVersionFilename(string version, string filename)
        {
            return new Uri(updateServerUrl, Path.Combine(version, filename)).ToString();
        }

        private string GetVersionFilename(string nameMode, string version, string filename)
        {
            return new Uri(updateServerUrl, Path.Combine("download", nameMode, version, filename)).ToString();
        }

        private string GetUpdateModeFilename(string nameMode, string version)
        {
            return new Uri(updateServerUrl, Path.Combine("download", "u", nameMode, version, "updates")).ToString();
        }

        private string GetModeVersionFilename(string key, string version, string filename, string modename)
        {
            return new Uri(updateServerUrl, Path.Combine(key, modename, version, filename)).ToString();
        }
    }
}
