/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 06 мая 2026 10:50:54
 * Version: 1.0.43
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

//using SevenZipExtractor;

namespace AppUpdater.Server
{
    public static class SevenZipHelper
    {
        /// <summary>
        /// Распаковывает 7z архив в указанную папку
        /// </summary>
        public static void FastExtract7zDeprecated(string archivePath, string outputDir, Action<int, int> progressCountUnpack)
        {
            //try
            //{
            //    using (var archive = new ArchiveFile(archivePath))
            //    {
            //        var outputDirFull = Path.GetFullPath(outputDir).TrimEnd(Path.DirectorySeparatorChar);
            //        Directory.CreateDirectory(outputDirFull);

            //        int totalFiles = archive.Entries.Count(e => !e.IsFolder);
            //        int countFiles = 0;

            //        archive.Extract(entry =>
            //        {
            //            var targetPath = Path.Combine(outputDirFull, entry.FileName ?? string.Empty);

            //            if (entry.IsFolder)
            //            {
            //                Directory.CreateDirectory(targetPath);
            //                return targetPath;
            //            }

            //            // Создаём директорию под файл
            //            var dir = Path.GetDirectoryName(targetPath);
            //            if (!string.IsNullOrEmpty(dir))
            //                Directory.CreateDirectory(dir);

            //            // Извлекаем файл в безопасном режиме
            //            try
            //            {
            //                // Удаляем старый файл, если есть
            //                if (File.Exists(targetPath))
            //                    File.Delete(targetPath);

            //                // Извлекаем через стандартный Extract
            //                entry.Extract(targetPath);
            //            }
            //            catch
            //            {
            //                // Игнорируем ошибки записи (можно логировать)
            //            }

            //            countFiles++;
            //            progressCountUnpack?.Invoke(countFiles, totalFiles);

            //            return targetPath;
            //        });
            //    }
            //}
            //catch (Exception ex)
            //{
            //    // Логировать ошибку, но не падаем
            //}
        }

        public static bool FastExtract7z(string archivePath, string outputDir)
        {
            string sevenZipExe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7za.exe");

            if (!File.Exists(sevenZipExe))
            {
                return false;
            }

            // x — extract, -o — output, -y — overwrite without prompt
            string args = $"x \"{archivePath}\" -o\"{outputDir}\" -y";

            var psi = new ProcessStartInfo
            {
                FileName = sevenZipExe,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (var proc = Process.Start(psi))
            {
                proc.WaitForExit(); // ждём завершения

                string output = proc.StandardOutput.ReadToEnd();
                string error = proc.StandardError.ReadToEnd();

                if (!string.IsNullOrWhiteSpace(error))
                    return false;
            }

            return true;
        }
    } 
}
