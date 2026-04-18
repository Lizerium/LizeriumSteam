/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 18 апреля 2026 15:00:38
 * Version: 1.0.25
 */


using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using AppUpdater.Log;
using AppUpdater.Manifest;
using AppUpdater.Server;

//using SevenZipExtractor;

namespace TarExample
{
    public class Tar
    {
        public EventHandler<UnpackHandle> DataUnpackHandle { get; set; }
        public EventHandler WarningLowTimeOperation { get; set; }
        public EventHandler WarningUnpack7zOperation { get; set; }
        public EventHandler SuccessUnpackZipOperation { get; set; }
        public EventHandler SuccessUnpackTarOperation { get; set; }
        
        /// <summary>
        /// Extracts a <i>.tar.gz</i> archive to the specified directory.
        /// </summary>
        /// <param name="filename">The <i>.tar.gz</i> to decompress and extract.</param>
        /// <param name="outputDir">Output directory to write the files.</param>
        public async void ExtractTarGz(string filename, string outputDir)
        {
            using (var stream = File.OpenRead(filename))
                await ExtractTarGz(stream, outputDir);
        }

        /// <summary>
        /// Extracts a <i>.tar.gz</i> archive stream to the specified directory.
        /// </summary>
        /// <param name="stream">The <i>.tar.gz</i> to decompress and extract.</param>
        /// <param name="outputDir">Output directory to write the files.</param>
        public async Task ExtractTarGz(Stream stream, string outputDir)
        {
            // A GZipStream is not seekable, so copy it first to a MemoryStream
            using (var gzip = new GZipStream(stream, CompressionMode.Decompress))
            {
                const int chunk = 4096;
                using (var memStr = new MemoryStream())
                {
                    int read;
                    var buffer = new byte[chunk];
                    do
                    {
                        read = gzip.Read(buffer, 0, chunk);
                        memStr.Write(buffer, 0, read);
                    } while (read == chunk);

                    memStr.Seek(0, SeekOrigin.Begin);
                    await ExtractTarAsync(memStr, outputDir);
                }
            }
        }

        /// <summary>
        /// DEPRECATED
        /// Extractes a <c>tar</c> archive to the specified directory.
        /// </summary>
        /// <param name="filename">The <i>.tar</i> to extract.</param>
        /// <param name="outputDir">Output directory to write the files.</param>
        public async Task ExtractTarWithInner7zAsync(string filename, string outputDir, ILog log)
        {
            //if (string.IsNullOrEmpty(filename) || string.IsNullOrEmpty(outputDir))
            //    return;

            //Directory.CreateDirectory(outputDir);

            //try
            //{
            //    log.Info($"Extract TAR: {filename} -> {outputDir}");

            //    // Распаковываем tar
            //    using (var stream = File.OpenRead(filename))
            //    {
            //        await ExtractTarStreamedAsync(stream, outputDir, log);
            //    }

            //    // Проверяем, есть ли 7z архивы внутри распакованного tar
            //    var inner7zFiles = Directory.GetFiles(outputDir, "*.7z", SearchOption.AllDirectories);

            //    int totalArchives = inner7zFiles.Length;
            //    int current = 0;

            //    foreach (var inner7z in inner7zFiles)
            //    {
            //        current++;

            //        log?.Info($"Extracting inner 7z: {inner7z}");

            //        // Определяем папку для распаковки
            //        var targetDir = Path.GetDirectoryName(inner7z);

            //        // Сначала проверяем, есть ли внутри манифест
            //        UpdateManifestModel innerManifest = null;
            //        using (var archive = new ArchiveFile(inner7z))
            //        {
            //            foreach (var entry in archive.Entries)
            //            {
            //                if (entry.FileName.EndsWith("manifest.json", StringComparison.OrdinalIgnoreCase))
            //                {
            //                    using (var ms = new MemoryStream())
            //                    {
            //                        entry.Extract(ms);
            //                        ms.Position = 0;
            //                        innerManifest = JsonSerializer.Deserialize<UpdateManifestModel>(ms);
            //                        log?.Info("[Inner Manifest] Parsed from 7z");
            //                    }
            //                    break;
            //                }
            //            }
            //        }

            //        // Создаём прогресс-делегат для FastExtract7z
            //        Action<int, int> progress = (done, total) =>
            //        {
            //            if (total > 0)
            //            {
            //                DataUnpackHandle?.Invoke(this, new UnpackHandle()
            //                {
            //                    TotalUnpackFiles = done,
            //                    CurrentUnpackFilesCount = total
            //                });

            //                log.Info($"7z progress: {done}/{total} - {Path.GetFileName(inner7z)}");
            //            }
            //        };

            //        // извлекаем
            //        SevenZipHelper.FastExtract7z(inner7z, targetDir);
            //        // подчищаем по манифесту
            //        DeletesToManifest(log, targetDir, innerManifest);
                    
            //        var manifestPath = Path.Combine(targetDir, "manifest.json");
            //        if (File.Exists(manifestPath))
            //            File.Delete(manifestPath);

            //        // После распаковки можно удалить исходный 7z
            //        File.Delete(inner7z);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    log.Error($"Error extracting TAR or inner 7z: {ex.Message}");
            //}
        }

        private void DeletesToManifest(ILog log, string targetDir, UpdateManifestModel innerManifest)
        {
            // Удаляем файлы/папки из внутреннего манифеста
            if (innerManifest != null)
            {
                int i = 0;
                foreach (var file in innerManifest.DeletedFiles)
                {
                    i++;
                    var fullPath = Path.Combine(targetDir, file);
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                        log?.Info($"[Deleted File] {file}");
                    }
                    log?.Info($"[DEBUG] send update: i={i}, fullPathExists={Directory.Exists(fullPath)}");
                    DataUnpackHandle?.Invoke(this, new UnpackHandle()
                    {
                        CurrentDeleteFilesCount = i,
                        TotalDeleteFiles = innerManifest.DeletedFiles.Count,
                    });

                    Thread.Sleep(100);
                }
                i = 0;
                foreach (var dir in innerManifest.DeletedDirectory.OrderByDescending(d => d.Length))
                {
                    i++;
                    var fullDir = Path.Combine(targetDir, dir);
                    if (Directory.Exists(fullDir))
                    {
                        Directory.Delete(fullDir, true);
                        log?.Info($"[Deleted Dir] {dir}");
                    }
                    log?.Info($"[DEBUG] send update: i={i}, fullDirExists={Directory.Exists(fullDir)}");
                    DataUnpackHandle?.Invoke(this, new UnpackHandle()
                    {
                        CurrentDeleteDirsCount = i,
                        TotalDeleteDirs = innerManifest.DeletedDirectory.Count,
                    });

                    Thread.Sleep(100);
                }

                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// DEPRECATED
        /// </summary>
        public async Task ExtractTarStreamedAsync(Stream stream, string outputDir, ILog log = null)
        {
            try
            {
                var buffer = new byte[512];
                UpdateManifestModel manifest = null;
                int currentFiles = 0;
                var filesToWrite = new List<(string Path, long Size, long DataStartPos)>();

                while (true)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, 512);
                    if (bytesRead < 512 || buffer.All(b => b == 0)) break;

                    var name = Encoding.ASCII.GetString(buffer, 0, 100).Trim('\0');
                    if (string.IsNullOrWhiteSpace(name)) break;

                    var sizeStr = Encoding.ASCII.GetString(buffer, 124, 12).Trim('\0').Trim();
                    var size = Convert.ToInt64(sizeStr, 8);

                    bool isDir = name.EndsWith("/") || (!name.Contains(".") && !name.Contains("\\"));
                    var outputPath = Path.Combine(outputDir, name);

                    if (isDir)
                    {
                        Directory.CreateDirectory(outputPath);
                    }
                    else
                    {
                        var outputFolder = Path.GetDirectoryName(outputPath);
                        if (!Directory.Exists(outputFolder))
                            Directory.CreateDirectory(outputFolder);

                        if (name.EndsWith("manifest.json", StringComparison.OrdinalIgnoreCase))
                        {
                            using (var ms = new MemoryStream())
                            {
                                await CopyStreamLimitedAsync(stream, ms, size);
                                ms.Position = 0;

                                manifest = await JsonSerializer.DeserializeAsync<UpdateManifestModel>(ms);
                                log?.Info("[Manifest] Parsed");
                            }
                        }
                        else
                        {
                            using (var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                            {
                                await CopyStreamLimitedAsync(stream, fs, size);
                                log?.Info($"[Extracted] {name}");
                                currentFiles++;
                                DataUnpackHandle?.Invoke(this, new UnpackHandle()
                                {
                                    TotalUnpackFiles = currentFiles,
                                    CurrentUnpackFilesCount = currentFiles
                                });
                            } 
                        }
                    }

                    var padding = 512 - (size % 512);
                    if (padding != 512)
                        stream.Seek(padding, SeekOrigin.Current);
                }

                // подчищаем по манифесту
                DeletesToManifest(log, outputDir, manifest);

                log?.Info("[Extract] Completed");
            }
            catch (Exception ex)
            {
                log?.Info("[Extract] Error!\n" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// DEPRECATED
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        private static async Task CopyStreamLimitedAsync(Stream input, Stream output, long limit)
        {
            byte[] buffer = new byte[8192];
            long totalRead = 0;
            while (totalRead < limit)
            {
                int toRead = (int)Math.Min(buffer.Length, limit - totalRead);
                int read = await input.ReadAsync(buffer, 0, toRead);
                if (read == 0) break;
                await output.WriteAsync(buffer, 0, read);
                totalRead += read;
            }
        }

        /// <summary>
        /// DEPRECATED
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="outputDir"></param>
        /// <param name="log"></param>
        public void ExtractTarStreamed(Stream stream, string outputDir, ILog log = null)
        {
            try
            {
                var buffer = new byte[512];
                UpdateManifestModel manifest = null;
                int currentFiles = 0;

                while (true)
                {
                    int bytesRead = stream.Read(buffer, 0, 512);
                    if (bytesRead < 512 || buffer.All(b => b == 0)) break;

                    var name = Encoding.ASCII.GetString(buffer, 0, 100).Trim('\0');
                    if (string.IsNullOrWhiteSpace(name)) break;

                    var sizeStr = Encoding.ASCII.GetString(buffer, 124, 12).Trim('\0').Trim();
                    long size = Convert.ToInt64(sizeStr, 8);

                    bool isDir = name.EndsWith("/") || (!name.Contains(".") && !name.Contains("\\"));
                    var outputPath = Path.Combine(outputDir, name);

                    if (isDir)
                    {
                        Directory.CreateDirectory(outputPath);
                    }
                    else
                    {
                        var outputFolder = Path.GetDirectoryName(outputPath);
                        if (!Directory.Exists(outputFolder))
                            Directory.CreateDirectory(outputFolder);

                        if (name.EndsWith("manifest.json", StringComparison.OrdinalIgnoreCase))
                        {
                            using (var ms = new MemoryStream())
                            {
                                CopyStreamLimited(stream, ms, size);
                                ms.Position = 0;
                                manifest = JsonSerializer.Deserialize<UpdateManifestModel>(ms);
                                log?.Info("[Manifest] Parsed");
                            }
                        }
                        else
                        {
                            using (var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                            {
                                CopyStreamLimited(stream, fs, size);
                                log?.Info($"[Extracted] {name}");
                                currentFiles++;
                                DataUnpackHandle?.Invoke(this, new UnpackHandle()
                                {
                                    TotalUnpackFiles = currentFiles,
                                    CurrentUnpackFilesCount = currentFiles
                                });
                            }
                        }
                    }

                    // корректируем padding до 512 байт
                    long padding = 512 - (size % 512);
                    if (padding != 512)
                        stream.Seek(padding, SeekOrigin.Current);
                }

                // подчищаем по манифесту
                DeletesToManifest(log, outputDir, manifest);

                log?.Info("[Extract] Completed");
            }
            catch (Exception ex)
            {
                log?.Info("[Extract] Error!\n" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// DEPRECATED
        /// Синхронная версия CopyStreamLimitedAsync
        /// </summary>
        private void CopyStreamLimited(Stream input, Stream output, long bytesToCopy)
        {
            var buffer = new byte[81920]; // 80 KB
            long remaining = bytesToCopy;

            while (remaining > 0)
            {
                int read = input.Read(buffer, 0, (int)Math.Min(buffer.Length, remaining));
                if (read <= 0) break;

                output.Write(buffer, 0, read);
                remaining -= read;
            }
        }

        /// <summary>
        /// DEPRECATED
        /// Extractes a <c>tar</c> archive to the specified directory.
        /// </summary>
        /// <param name="filename">The <i>.tar</i> to extract.</param>
        /// <param name="outputDir">Output directory to write the files.</param>
        public static void ExtractTar(string filename, string outputDir)
        {
            if (string.IsNullOrEmpty(filename)
                || string.IsNullOrEmpty(outputDir)) return;

            using (var stream = File.OpenRead(filename))
                ExtractTar(stream, outputDir);
        }

        /// <summary>
        /// DEPRECATED
        /// Extractes a <c>tar</c> archive to the specified directory.
        /// </summary>
        /// <param name="stream">The <i>.tar</i> to extract.</param>
        /// <param name="outputDir">Output directory to write the files.</param>
        public async Task ExtractTarAsync(Stream stream, string outputDir)
        {
            var buffer = new byte[100];
            while (true)
            {
                await stream.ReadAsync(buffer, 0, 100);
                var name = Encoding.ASCII.GetString(buffer).Trim('\0');
                if (String.IsNullOrWhiteSpace(name))
                    break;
                stream.Seek(24, SeekOrigin.Current);
                await stream.ReadAsync(buffer, 0, 12);
                var size = Convert.ToInt64(Encoding.UTF8.GetString(buffer, 0, 12).Trim('\0').Trim(), 8);

                stream.Seek(376L, SeekOrigin.Current);

                var output = Path.Combine(outputDir, name);
                if (!Directory.Exists(Path.GetDirectoryName(output)))
                    Directory.CreateDirectory(Path.GetDirectoryName(output));

                var t1 = name.Length;
                var t2 = name.LastIndexOf("/") + 1;

                if (name.Contains("/") &&  t1 <= t2 && !name.Contains(".")) continue;

                if (!name.Equals("./", StringComparison.InvariantCulture))
                {
                    if (File.Exists(output))
                        File.Delete(output);
                    using (var str = File.Open(output, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        var buf = new byte[size];
                        await stream.ReadAsync(buf, 0, buf.Length);
                        await str.WriteAsync(buf, 0, buf.Length);
                    }
                }

                var pos = stream.Position;

                var offset = 512 - (pos % 512);
                if (offset == 512)
                    offset = 0;

                stream.Seek(offset, SeekOrigin.Current);
            }
        }

        /// <summary>
        /// DEPRECATED
        /// Extractes a <c>tar</c> archive to the specified directory.
        /// </summary>
        /// <param name="stream">The <i>.tar</i> to extract.</param>
        /// <param name="outputDir">Output directory to write the files.</param>
        public static void ExtractTar(Stream stream, string outputDir)
        {
            var buffer = new byte[100];
            while (true)
            {
                stream.Read(buffer, 0, 100);
                var name = Encoding.ASCII.GetString(buffer).Trim('\0');
                if (String.IsNullOrWhiteSpace(name))
                    break;
                stream.Seek(24, SeekOrigin.Current);
                stream.Read(buffer, 0, 12);
                var size = Convert.ToInt64(Encoding.UTF8.GetString(buffer, 0, 12).Trim('\0').Trim(), 8);

                stream.Seek(376L, SeekOrigin.Current);

                var output = Path.Combine(outputDir, name);
                if (!Directory.Exists(Path.GetDirectoryName(output)))
                    Directory.CreateDirectory(Path.GetDirectoryName(output));

                var t1 = name.Length;
                var t2 = name.LastIndexOf("/") + 1;

                if (name.Contains("/") && t1 <= t2 && !name.Contains(".")) continue;

                if (!name.Equals("./", StringComparison.InvariantCulture))
                {
                    if (File.Exists(output))
                        File.Delete(output);
                    using (var str = File.Open(output, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        var buf = new byte[size];
                        stream.Read(buf, 0, buf.Length);
                        str.Write(buf, 0, buf.Length);
                    }
                }

                var pos = stream.Position;

                var offset = 512 - (pos % 512);
                if (offset == 512)
                    offset = 0;

                stream.Seek(offset, SeekOrigin.Current);
            }
        }

        /// <summary>
        /// DEPRECATED
        /// </summary>
        public void ExtractTarWithInner7zBlocking(string filename, string outputDir, ILog log)
        {
            //if (string.IsNullOrEmpty(filename) || string.IsNullOrEmpty(outputDir))
            //    return;

            //Directory.CreateDirectory(outputDir);

            //try
            //{
            //    log.Info($"Extract TAR: {filename} -> {outputDir}");

            //    // 1. Распаковка TAR
            //    using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            //    {
            //        ExtractTarStreamed(stream, outputDir, log); // синхронная версия
            //    }

            //    // 2. Проверка и распаковка всех 7z
            //    var inner7zFiles = Directory.GetFiles(outputDir, "*.7z", SearchOption.AllDirectories);

            //    foreach (var inner7z in inner7zFiles)
            //    {
            //        log?.Info($"Extracting inner 7z: {inner7z}");

            //        var targetDir = Path.GetDirectoryName(inner7z);

            //        // Парсим внутренний манифест
            //        UpdateManifestModel innerManifest = null;
            //        using (var archive = new ArchiveFile(inner7z))
            //        {
            //            foreach (var entry in archive.Entries)
            //            {
            //                if (entry.FileName.EndsWith("manifest.json", StringComparison.OrdinalIgnoreCase))
            //                {
            //                    using (var ms = new MemoryStream())
            //                    {
            //                        entry.Extract(ms);
            //                        ms.Position = 0;
            //                        innerManifest = JsonSerializer.Deserialize<UpdateManifestModel>(ms);
            //                        log?.Info("[Inner Manifest] Parsed from 7z");
            //                    }
            //                    break;
            //                }
            //            }
            //        }

            //        // Прогресс-делегат
            //        Action<int, int> progress = (done, total) =>
            //        {
            //            if (total > 0)
            //            {
            //                DataUnpackHandle?.Invoke(this, new UnpackHandle
            //                {
            //                    TotalUnpackFiles = done,
            //                    CurrentUnpackFilesCount = total
            //                });

            //                log?.Info($"7z progress: {done}/{total} - {Path.GetFileName(inner7z)}");
            //            }
            //        };

            //        // Извлечение внутреннего 7z без блокировки
            //        SevenZipHelper.FastExtract7zDeprecated(inner7z, targetDir, progress);

            //        // Подчищаем файлы по внутреннему манифесту
            //        DeletesToManifest(log, targetDir, innerManifest);

            //        // Удаляем manifest.json если остался
            //        var manifestPath = Path.Combine(targetDir, "manifest.json");
            //        if (File.Exists(manifestPath))
            //        {
            //            try { File.Delete(manifestPath); } catch { /* не критично */ }
            //        }

            //        // Удаляем исходный 7z
            //        try { File.Delete(inner7z); } catch { /* не критично */ }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    log.Error($"Error extracting TAR or inner 7z: {ex.Message}");
            //}
        }

        public void ExtractTarWithInnerZipFast(string filename, string outputDir, ILog log)
        {
            if (string.IsNullOrEmpty(filename) || string.IsNullOrEmpty(outputDir))
                return;

            Directory.CreateDirectory(outputDir);

            try
            {
                log.Info($"Extract TAR: {filename} -> {outputDir}");

                WarningLowTimeOperation.Invoke(this, EventArgs.Empty);
                // 1. Распаковка TAR безопасно, без блокировки файлов
                using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    ExtractTarStreamed(stream, outputDir, log); // синхронная версия
                }
                SuccessUnpackTarOperation(this, EventArgs.Empty);

                // 2. Поиск всех внутренних zip файлов
                var innerZipFiles = Directory.GetFiles(outputDir, "*.zip", SearchOption.AllDirectories);

                if(innerZipFiles.Length > 0)
                    WarningUnpack7zOperation.Invoke(this, EventArgs.Empty);
                // Распаковка zip в параллельных потоках
                Parallel.ForEach(innerZipFiles, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, innerZip =>
                {
                    var targetDir = Path.GetDirectoryName(innerZip);
                    log?.Info($"Extracting inner zip: {innerZip}");

                    // Прогресс-делегат
                    Action<int, int> progress = (done, total) =>
                    {
                        if (total > 0)
                        {
                            DataUnpackHandle?.Invoke(this, new UnpackHandle
                            {
                                TotalUnpackFiles = done,
                                CurrentUnpackFilesCount = total
                            });
                            log?.Info($"zip progress: {done}/{total} - {Path.GetFileName(innerZip)}");
                        }
                    };

                    // Параллельная распаковка внутреннего zip
                    try
                    {
                        ExtractZip.FastExtractZip(innerZip, targetDir, progress);
                    }
                    catch (Exception ex)
                    {
                        log?.Error($"Error extracting zip {innerZip}: {ex.Message}");
                    }

                    // 3. Только теперь ищем manifest.json
                    var manifestPath = Path.Combine(outputDir, "manifest.json");
                    try
                    {
                        if (File.Exists(manifestPath))
                        {
                            var fs = File.OpenRead(manifestPath);
                            var manifest = JsonSerializer.Deserialize<UpdateManifestModel>(fs);
                            // подчищаем по манифесту
                            DeletesToManifest(log, outputDir, manifest);
                            fs.Dispose();
                        }
                    }
                    catch(Exception ex)
                    {
                        log?.Error($"Error parse manifest.json to zip: {ex.Message}");
                    }

                    // Удаляем manifest.json если остался
                    try
                    {
                        if (File.Exists(manifestPath))
                            File.Delete(manifestPath);
                    }
                    catch (Exception ex)
                    {
                        log?.Error($"Error delete manifest.json to zip: {ex.Message}");
                    }

                    // Удаляем исходный zip
                    try
                    {
                        if (File.Exists(innerZip))
                            File.Delete(innerZip); 
                    }
                    catch (Exception ex)
                    {
                        log?.Error($"Error delete inner zip: {ex.Message}");
                    }
                });
               
                if (innerZipFiles.Length > 0)
                    SuccessUnpackZipOperation.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                log.Error($"Error extracting TAR or inner zip: {ex.Message}");
            }
        }

        /// <summary>
        /// DEPRECATED
        /// </summary>
        public void FastExtract7zParallel(string archivePath, string outputDir, Action<int, int> progress, ILog log)
        {
            //var archive = new ArchiveFile(archivePath);
            //var entries = archive.Entries.Where(e => !e.IsFolder).ToArray();
            //int total = entries.Length;
            //int count = 0;

            //Parallel.ForEach(entries, new ParallelOptions { MaxDegreeOfParallelism = Math.Min(4, Environment.ProcessorCount) }, entry =>
            //{
            //    var targetPath = Path.Combine(outputDir, entry.FileName);
            //    Directory.CreateDirectory(Path.GetDirectoryName(targetPath));

            //    try
            //    {
            //        // SharpCompress/SevenZipExtractor: Extract directly to file
            //        using (var fs = new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            //        {
            //            entry.Extract(fs); // вот так работает для 7z
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        log.Error($"Failed extract {entry.FileName}: {ex.Message}");
            //    }

            //    Interlocked.Increment(ref count);
            //    progress?.Invoke(count, total);
            //});
            
            //archive.Dispose();
        }
    }
}

