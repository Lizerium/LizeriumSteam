/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 31 марта 2026 11:07:39
 * Version: 1.0.4
 */

using AppUpdater.LocalStructure;
using AppUpdater.Recipe;
using AppUpdater.Server;
using AppUpdater.Log;
using AppUpdater.Utils;
using System.Linq;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AppUpdater.LocalStructure.Components;

namespace AppUpdater.Chef
{
    public class UpdaterChef : IUpdaterChef
    {
        private readonly ILog log = Logger.For<UpdaterChef>();
        private readonly ILocalStructureManager localStructureManager;
        private readonly IUpdateServer updateServer;
        private bool IsProcessCook = false;

        public UpdaterChef(ILocalStructureManager localStructureManager, IUpdateServer updateServer)
        {
            this.localStructureManager = localStructureManager;
            this.updateServer = updateServer;
        }

        public EventHandler<double> PercentUpdate { get; set; }
        public EventHandler<StateErrorUpdate> EndCook { get; set; }
        public EventHandler<UpdateCountFilesHandler> UpdateCountFilesHandler { get; set; }
        public EventHandler<StateErrorUpdate> ErrorUpdateLauncher { get; set; }

        public async Task<StateErrorUpdate> Cook(UpdateRecipe recipe, CancellationToken token)
        {
            if (localStructureManager.HasVersionFolder(recipe.NewVersion))
            {
                localStructureManager.DeleteVersionDir(recipe.NewVersion);
            }

            localStructureManager.CreateVersionDir(recipe.NewVersion);

            var countFiles = 0;
            var maxCount = recipe.Files.ToList().Count;
         
            foreach (var file in recipe.Files)
            {
                countFiles++;
                double percentage = (double)countFiles / maxCount * 100;

                PercentUpdate?.Invoke(this, percentage);
                UpdateCountFilesHandler?.Invoke(this, new UpdateCountFilesHandler()
                {
                    CurrentCountFiles = countFiles,
                    MaxCountFiles = maxCount
                });

                if (file.Action == FileUpdateAction.Copy)
                {
                    log.Debug("Copying file \"{0}\" from version \"{1}\".", file.Name, recipe.CurrentVersion);
                    localStructureManager.CopyFile(recipe.CurrentVersion, recipe.NewVersion, file.Name);
                }
                else if (file.Action == FileUpdateAction.Download)
                {
                    log.Debug("Downloading file \"{0}\".", file.FileToDownload);
                    byte[] data = await updateServer.DownloadFile(token, recipe.NewVersion, file.FileToDownload);
                    log.Debug("Decompressing the file.");
                    data = DataCompressor.Decompress(data);
                    log.Debug("Saving the file \"{0}\".", file.Name);
                    StateSaveFile state = localStructureManager.SaveFile(recipe.NewVersion, file.Name, data);
                    if (state.State == false)
                        return StateErrorUpdate.ErrorSaveFile;

                    if(!string.IsNullOrEmpty(state.NameFile)
                        && state.NameFile == "config.xml")
                    {
                        log.Debug($"config.xml is exist an update: copy new keys started from {state.PathFile} to original");
                        localStructureManager.CompareConfigs(state);
                    }
                }
                else if (file.Action == FileUpdateAction.DownloadDelta)
                {
                    log.Debug("Downloading patch file \"{0}\".", file.FileToDownload);
                    byte[] data = await updateServer.DownloadFile(token, recipe.NewVersion, file.FileToDownload);
                    log.Debug("Applying patch file.");
                    localStructureManager.ApplyDelta(recipe.CurrentVersion, recipe.NewVersion, file.Name, data);
                }
            }

            return StateErrorUpdate.Success;
        }

        public void EndCookEvent(StateErrorUpdate state)
        {
            EndCook?.Invoke(this, state);
        }

        public async void CookMode(CancellationToken token, UpdateRecipe recipe, string Mode)
        {
            var pathModeDownload = Path.Combine(Path.GetTempPath(), Mode);
            if (!Directory.Exists(pathModeDownload))
                Directory.CreateDirectory(pathModeDownload);

            var countFiles = 0;
            var maxCount = recipe.Files.ToList().Count;

            foreach (var file in recipe.Files)
            {
                countFiles++;
                UpdateCountFilesHandler?.Invoke(this, new UpdateCountFilesHandler()
                {
                    CurrentCountFiles = countFiles,
                    MaxCountFiles = maxCount
                });
                double percentage = (double)countFiles / maxCount * 100;
                PercentUpdate?.Invoke(this, percentage);

                if (file.Action == FileUpdateAction.Download)
                {
                    log.Debug("Downloading file \"{0}\".", file.FileToDownload);
                    byte[] data = await updateServer.DownloadFile(token, Mode, recipe.NewVersion, file.FileToDownload);
                    log.Debug("Decompressing the file.");
                    data = DataCompressor.Decompress(data);
                    log.Debug("Saving the file \"{0}\".", file.Name);
                    var filename = file.Name;
                    var destinationFilename = string.Empty;

                    if (!Directory.Exists(pathModeDownload))
                    {
                        var index_1 = filename.IndexOf('\\') + 1;
                        var lenght = filename.Length - filename.IndexOf('\\') - 1;
                        var newName = filename.Substring(index_1, lenght);
                        destinationFilename = newName;
                    }

                    File.WriteAllBytes(destinationFilename, data);
                }
                else if (file.Action == FileUpdateAction.DownloadDelta)
                {
                    log.Debug("Downloading patch file \"{0}\".", file.FileToDownload);
                    var path = Path.Combine(Path.GetTempPath(), Mode);
                    var pathFile = Path.Combine(path, file.FileToDownload);
                    var TokenSource = new CancellationTokenSource();
                    string newPath = Path.ChangeExtension(pathFile, null);
                    string fileRes = Path.ChangeExtension(file.FileToDownload, null);
                    await updateServer.DownloadAndSaveFile(token, path, Mode, recipe.NewVersion, fileRes);
                    long totalMemory = GC.GetTotalMemory(false);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }

            if (!token.IsCancellationRequested)
                EndCook.Invoke(this, StateErrorUpdate.Success);
        }

        public static byte[] ReadFileInChunks(string filePath)
        {
            // Calculate chunk size (adjust as needed)
            int chunkSize = 32768;

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                // Calculate the total file size
                long totalFileSize = fileStream.Length;

                // Create a MemoryStream to store the data
                using (var memoryStream = new MemoryStream())
                {
                    // Read data in chunks
                    byte[] buffer = new byte[chunkSize];
                    int bytesRead;
                    while ((bytesRead = fileStream.Read(buffer, 0, chunkSize)) > 0)
                    {
                        memoryStream.Write(buffer, 0, bytesRead);
                    }
                    return memoryStream.ToArray();
                }
            }
        }
    }
}
