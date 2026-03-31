/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 31 марта 2026 11:07:39
 * Version: 1.0.4
 */

using System.IO;
using AppUpdater.LocalStructure;
using AppUpdater.Manifest;
using AppUpdater.Recipe;
using AppUpdater.Server;
using AppUpdater.Chef;
using AppUpdater.Log;
using System.Linq;
using System;
using AppUpdater.Delta;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using AppUpdater.LocalStructure.Components;

namespace AppUpdater
{
    public class UpdateManager : IUpdateManager
    {
        public readonly IUpdateServer UpdateServer;
        private ILog log = Logger.For<UpdateManager>();
        private readonly ILocalStructureManager localStructureManager;
        private readonly IUpdaterChef updaterChef;

        public string CurrentVersion { get; private set; }

        private static UpdateManager defaultInstance;

        #region Update Handle
        public EventHandler<UpdateCountFilesHandler> UpdateAppCountHandler;
        public EventHandler<StateErrorUpdate> EndUpdateAppHandler;
        public StateErrorUpdate StateCook {  get; set; }
        #endregion

        public static UpdateManager Default
        {
            get
            {
                if (defaultInstance == null)
                {
                    string baseDir = Path.Combine(Path.GetDirectoryName(typeof(UpdateManager).Assembly.Location), "..\\");
                    ILocalStructureManager manager = new DefaultLocalStructureManager(baseDir);
                    IUpdateServer updateServer = new DefaultUpdateServer(manager.GetUpdateServerUri());
                    defaultInstance = new UpdateManager(updateServer, manager, new UpdaterChef(manager, updateServer));
                    defaultInstance.Initialize();
                }

                return defaultInstance;
            }
        }

        public EventHandler<double> PercentUpdate { get; set; }
        public EventHandler<StateErrorUpdate> EndCookHandle { get; set; }
        public EventHandler<UpdateCountFilesHandler> UpdateCountFilesHandler { get; set; }
        public EventHandler<StateErrorUpdate> StateErrorUpdateLauncherHandler { get; set; }
        public EventHandler<DataDownloadHandle> DataDownloadHandle;
        public EventHandler<UnpackHandle> DataUnpackHandle;
        public EventHandler<EventArgs> SuccessUnpackTarOperationHandle;
        public EventHandler<EventArgs> SuccessUnpack7zOperationHandle;
        public EventHandler<EventArgs> WarningUnpack7zOperationHandle;
        public EventHandler<EventArgs> WarningLowTimeOperationHandle;

        public UpdateManager(string BaseDir)
        {
            Init(BaseDir);
        }

        private void Init(string BaseDir)
        {
            ILocalStructureManager manager = new DefaultLocalStructureManager(BaseDir);
            IUpdateServer updateServer = new DefaultUpdateServer(manager.GetUpdateServerUri());
            var updaterChef = new UpdaterChef(manager, updateServer);
            defaultInstance = new UpdateManager(updateServer, manager, updaterChef);
            defaultInstance.Initialize();
        }

        private void UpdateCountFiles(object sender, UpdateCountFilesHandler e)
        {
            UpdateCountFilesHandler?.Invoke(sender, e);
        }

        public UpdateManager(IUpdateServer updateServer, ILocalStructureManager localStructureManager, IUpdaterChef updaterChef)
        {
            updateServer.UpdateDownloadHandle = null;
            updaterChef.UpdateCountFilesHandler = null;
            updateServer.UpdateDownloadHandle += UpdateDownloadHandle;
            updaterChef.UpdateCountFilesHandler += UpdateCountFiles;

            this.UpdateServer = updateServer;
            this.localStructureManager = localStructureManager;
            this.updaterChef = updaterChef;
        }

        private void UpdateDownloadHandle(object sender, DataDownloadHandle e)
        {
            DataDownloadHandle?.Invoke(this, e);
        }

        private void DataUnpackHandleLogic(object sender, UnpackHandle e)
        {
            DataUnpackHandle.Invoke(this, e);
        }

        private void SuccessUnpackTarOperationLogic(object sender, EventArgs e)
        {
            SuccessUnpackTarOperationHandle?.Invoke(this, e);
        }

        private void SuccessUnpack7zOperationLogic(object sender, EventArgs e)
        {
            SuccessUnpack7zOperationHandle?.Invoke(this, e);
        }

        private void WarningUnpack7zOperationLogic(object sender, EventArgs e)
        {
            WarningUnpack7zOperationHandle?.Invoke(this, e);
        }

        private void WarningLowTimeOperationLogic(object sender, EventArgs e)
        {
            WarningLowTimeOperationHandle?.Invoke(this, e);
        }


        public void Initialize()
        {
            this.CurrentVersion = localStructureManager.GetCurrentVersion();
        }

        public async Task<UpdateInfo> CheckForUpdate()
        {
            string serverCurrentVersion = await UpdateServer.GetCurrentVersion();
            if (string.IsNullOrEmpty(serverCurrentVersion))
            {
                var info = new UpdateInfo(false, serverCurrentVersion);
                info.NotAwailableServer = true;
                return info;
            }
            bool hasUpdate = CurrentVersion != serverCurrentVersion;
            if (string.IsNullOrEmpty(serverCurrentVersion)) hasUpdate = false;
            return new UpdateInfo(hasUpdate, serverCurrentVersion);
        }

        /// <summary>
        /// Проверяет старый конфиг и конфиг последнего обновления на наличие отличий
        /// </summary>
        /// <returns></returns>
        public StateCheckUpdateConfigXML CheckAndCompareConfigs()
        {
            var currentVersion = localStructureManager.GetExecutingVersion();
            var dirLastUpdate = Path.Combine(localStructureManager.GetBaseDir(), currentVersion);
            if (Directory.Exists(dirLastUpdate)) {
                var newConfigPath = Path.Combine(dirLastUpdate, "config.xml");
                if (File.Exists(newConfigPath))
                {
                    var resultMerge = localStructureManager.CompareConfigs(new StateSaveFile()
                    {
                        NameFile = "config.xml",
                        PathFile = newConfigPath,
                        State = true
                    });

                    if (resultMerge.Result.Count > 0)
                    {
                        return new StateCheckUpdateConfigXML()
                        {
                            State = true,
                            TextMessage = $"{newConfigPath} обнаружены обновления!\n" + string.Join("\n", resultMerge.Result.Select(it => it.ToString()))
                        };
                    }
                    else
                    {
                        return new StateCheckUpdateConfigXML()
                        {
                            State = true,
                            TextMessage = $"{newConfigPath} успешно проверены! Разницы не обнаружено!"
                        };
                    }
                }
                else
                    return new StateCheckUpdateConfigXML()
                    {
                        State = false,
                        TextMessage = $"{newConfigPath} не существует такого файла в последнем обновлении!"
                    };
            }
            else
                return new StateCheckUpdateConfigXML()
                {
                    State = false,
                    TextMessage = $"{dirLastUpdate} не существует такой директории!"
                };
        }

        public async Task DoUpdate(UpdateInfo updateInfo, CancellationToken token)
        {
            VersionManifest currentVersionManifest = localStructureManager.LoadManifest(this.CurrentVersion);
            VersionManifest newVersionManifest = await UpdateServer.GetManifest(updateInfo.Version);
            UpdateRecipe recipe = currentVersionManifest.UpdateTo(newVersionManifest);

            updaterChef.PercentUpdate = null;
            updaterChef.PercentUpdate += UpdatePercent;
            StateCook = await updaterChef.Cook(recipe, token);

            // пишем в наш конфигурационный файл прошлый номер версии
            localStructureManager.SetLastValidVersion(localStructureManager.GetExecutingVersion());
            // пишем в наш конфигурационный файл номер новой текущей версии
            localStructureManager.SetCurrentVersion(updateInfo.Version);
            // добавляем недостающие ключи XML в наш текущий конфигурационный файл

            CurrentVersion = updateInfo.Version;

            DeleteOldVersions();
        }

        public static List<string> GenerateVersions(string start, string stop)
        {
            // Разбиваем строки на части
            string[] startParts = start.Split('.');
            string[] stopParts = stop.Split('.');

            // Проверяем, что строки имеют корректный формат
            if (startParts.Length != 3 || stopParts.Length != 3)
            {
                return new List<string>();
            }

            // Преобразуем части в целые числа
            int start1 = int.Parse(startParts[0]);
            int start2 = int.Parse(startParts[1]);
            int start3 = int.Parse(startParts[2]);
            int stop1 = int.Parse(stopParts[0]);
            int stop2 = int.Parse(stopParts[1]);
            int stop3 = int.Parse(stopParts[2]);

            // Создаем список версий
            List<string> versions = new List<string>();

            // Цикл по частям строки
            for (int i = start1; i <= stop1; i++)
            {
                for (int j = (i == start1 ? start2 : 0); j <= (i == stop1 ? stop2 : 9); j++)
                {
                    for (int k = (i == start1 && j == start2 ? start3 : 0); k <= (i == stop1 && j == stop2 ? stop3 : 9); k++)
                    {
                        // Добавляем версию в список
                        versions.Add($"{i}.{j}.{k}");
                    }
                }
            }

            return versions;
        }

        public void ClearUpdateDir(string directory)
        {
            if (!Directory.Exists(directory)) return;

            DirectoryInfo di = new DirectoryInfo(directory);

            foreach (FileInfo file in di.GetFiles())
            {
                try
                {
                    file.Delete();
                }
                finally {  }
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                try
                {
                    dir.Delete(true);
                }
                finally { }
            }
        }

        public async Task<Exception> GetUpdatesMode(CancellationToken token, string oldVersion, string newVersion, string modeName, string dir)
        {
            var clearAllFilesPath = Path.Combine(Path.GetTempPath(), modeName, "updates");

            try
            {
                List<string> versions = GenerateVersions(oldVersion, newVersion);
                if (versions != null && versions.Count > 1)
                    versions.RemoveAt(0);
                ClearUpdateDir(clearAllFilesPath);

                int countUpdate = 0;
                foreach (var v in versions)
                {
                    token.ThrowIfCancellationRequested(); // проверка отмены

                    countUpdate++;
                    UpdateAppCountHandler.Invoke(this, new UpdateCountFilesHandler()
                    {
                        CurrentCountFiles = countUpdate,
                        MaxCountFiles = versions.Count
                    });
                    UpdateServer.UpdateDownloadHandle = null;
                    UpdateServer.UpdateDownloadHandle += UpdateDownloadHandle;
                    var downloadAndSave = await UpdateServer.DownloadUpdateFile(token, modeName, v, dir);
                }

                var tarExtractor = new TarExample.Tar();

                int countUnpack = 0;
                foreach (var v in versions)
                {
                    token.ThrowIfCancellationRequested(); // проверка отмены

                    countUnpack++;
                    var dirPack = Path.Combine(Path.GetTempPath(), modeName, "updates");
                    var pathUpdate = Path.Combine(dirPack, v + ".tar");
                    if (!File.Exists(pathUpdate)) continue;
                    try
                    {
                        tarExtractor.DataUnpackHandle = null;
                        tarExtractor.DataUnpackHandle += DataUnpackHandleLogic;
                        tarExtractor.WarningLowTimeOperation += WarningLowTimeOperationLogic;
                        tarExtractor.WarningUnpack7zOperation += WarningUnpack7zOperationLogic;
                        tarExtractor.SuccessUnpackZipOperation += SuccessUnpack7zOperationLogic;
                        tarExtractor.SuccessUnpackTarOperation += SuccessUnpackTarOperationLogic;
                        await Task.Run(() => tarExtractor.ExtractTarWithInnerZipFast(pathUpdate, dir, log));
                    }
                    catch (Exception ex)
                    {
                        log.Error("[ERROR] РАСПАКОВКА НЕ СУЩЕСТВУЮЩЕГО АРХИВА::", ex.Message, ex.StackTrace);
                        ClearUpdateDir(clearAllFilesPath);
                        EndUpdateAppHandler.Invoke(this, StateErrorUpdate.ErrorUnpackFile);
                        return ex;
                    }
                }

                ClearUpdateDir(clearAllFilesPath);
                EndUpdateAppHandler.Invoke(this, StateErrorUpdate.Success);
                return null;
            }
            catch (OperationCanceledException)
            {
                ClearUpdateDir(clearAllFilesPath);
                EndUpdateAppHandler.Invoke(this, StateErrorUpdate.CancelUpdate);
                return null;
            }
            catch (Exception ex) 
            {
                return ex;
            }
        }

        public void StopInstallMode()
        {
            updaterChef.EndCookEvent(StateErrorUpdate.CancelUpdate);
        }

        public async Task<bool> DoInstallMode(CancellationToken token, UpdateInfo updateInfo, string oldVersion, string newVersion, string modeName)
        {
            VersionManifest VersionManifest = await UpdateServer.GetModeManifest(modeName, updateInfo.Version);
            if (VersionManifest == null) return false;

            List<UpdateRecipeFile> recipeFiles = new List<UpdateRecipeFile>();
            updaterChef.EndCook = null;
            updaterChef.EndCook += EndCook;

            foreach (var file in VersionManifest.Files)
            {
                if(token.IsCancellationRequested) continue;

                FileUpdateAction action = FileUpdateAction.Download;
                string fileToDownload = file.DeployedName;
                long size = file.Size;
                if (DeltaAPI.IsSupported())
                {
                    VersionManifestDeltaFile delta = file.GetDeltaFrom(file.Checksum);
                    if (delta != null)
                    {
                        action = FileUpdateAction.DownloadDelta;
                        fileToDownload = delta.Filename;
                        size = delta.Size;
                    }
                }

                recipeFiles.Add(new UpdateRecipeFile(file.Name, file.Checksum, size, FileUpdateAction.DownloadDelta, fileToDownload));
            }

            UpdateRecipe recipe = new UpdateRecipe(newVersion, oldVersion, recipeFiles);
            updaterChef.CookMode(token, recipe, modeName);
            return true;
        }

        private void EndCook(object sender, StateErrorUpdate e)
        {
            EndCookHandle?.Invoke(this, e);
        }

        private void UpdatePercent(object sender, double percent)
        {
            PercentUpdate.Invoke(this, percent);
        }

        private void DeleteOldVersions()
        {
            string executingVersion = localStructureManager.GetExecutingVersion();
            string[] installedVersions = localStructureManager.GetInstalledVersions();
            string[] versionsInUse = new string[] { executingVersion, CurrentVersion };

            foreach (var version in installedVersions.Except(versionsInUse))
            {
                try
                {
                    localStructureManager.DeleteVersionDir(version);
                }
                catch (Exception err)
                {
                    log.Error("Error deleting old version ({0}). {1}", version, err.Message);
                }
            }
        }
    }
}
