/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 04 мая 2026 07:13:51
 * Version: 1.0.41
 */

using AppUpdater.Manifest;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AppUpdater.Server
{
    public interface IUpdateServer
    {
        Task<string> GetCurrentVersion();
        string GetLastVersionMode(string nameMode);
        string GetCurrentUpdateMode(string nameMode);
        Task<string> GetCurrentUpdateModeAsync(string nameMode);
        Task<VersionManifest> GetManifest(string version);
        Task<VersionManifest> GetModeManifest(string ModeName, string version);
        Task<byte[]> DownloadFile(CancellationToken token, string version, string filename);
        Task<byte[]> DownloadFile(CancellationToken token, string nameMode, string version, string filename);
        Task<byte[]> DownloadUpdateFile(CancellationToken token, string nameMode, string version, string directory);
        Task<byte[]> DownloadAndSaveFile(CancellationToken token, string path, string mode, string newVersion, string fileToDownload);
        string GetURL(string path, string mode, string newVersion, string fileToDownload);

        EventHandler<DataDownloadHandle> UpdateDownloadHandle { get; set; }
        EventHandler<string> VersionReceive { get; set; }
    }
}
