/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 31 марта 2026 11:07:39
 * Version: 1.0.4
 */

using System;
using System.IO;
using System.IO.Compression;

namespace AppUpdater.Server
{
    public static class ExtractZip
    {
        public static void FastExtractZip(string archivePath, string outputDir, Action<int, int> progress = null)
        {
            using (ZipArchive archive = ZipFile.OpenRead(archivePath))
            {
                int total = archive.Entries.Count;
                int current = 0;

                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string fullPath = Path.Combine(outputDir, entry.FullName);

                    // Проверяем, это папка или файл
                    if (string.IsNullOrEmpty(entry.Name))
                    {
                        Directory.CreateDirectory(fullPath);
                    }
                    else
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                        entry.ExtractToFile(fullPath, overwrite: true);
                    }

                    current++;
                    progress?.Invoke(current, total);
                }
            }
        }
    }
}
