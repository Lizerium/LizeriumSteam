/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 07 мая 2026 15:46:52
 * Version: 1.0.44
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Markup;

using LizeriumSteam.Models.Games;
using LizeriumSteam.Services.Games.ProjectMonitorService;
using LizeriumSteam.Services.Games.ProjectMonitorService.Components;
using LizeriumSteam.Services.Settings;

using Microsoft.Extensions.Logging;

namespace LizeriumSteam.Services.Games.GameSaveManagerService.Implennts
{
    /// <summary>
    /// Cервис загрузки и создания сохранений проектов
    /// </summary>
    public class GameSaveManagerService : IGameSaveManagerService
    {
        private readonly IProjectActivityMonitor _projectActivityMonitor;
        private readonly Logger<GameSaveManagerService> _logger;
        private readonly IAppSettings _appSettings;

        public GameSaveManagerService(Logger<GameSaveManagerService> logger,
            IProjectActivityMonitor projectActivityMonitor,
            IAppSettings appSettings)
        {
            _projectActivityMonitor = projectActivityMonitor;
            _logger = logger;
            _appSettings = appSettings;
        }

        /// <summary>
        /// Сохраняет данные которые остались от другого проекта не зарегистрированного в лаунчере
        /// </summary>
        /// <param name="gameSaveFolder">Источник данных</param>
        public void BackupUncnownSaveData(string gameSaveFolder, string extraBackupFolder)
        {
            // Создать extraBackupFolder и временную папку для хранения сохранения
            var pathExtra = Path.Combine(extraBackupFolder, DateTime.Now.ToString("dd.MM.yyyy.hh.mm.ss"));
            if (!Directory.Exists(pathExtra))
                Directory.CreateDirectory(pathExtra);
            // Сделать бэкап текущих неизвестных сохранений
            BackupFolder(gameSaveFolder, pathExtra, clearBackup: false);
            // Очистить папку MyGames
            ClearFolder(gameSaveFolder);
        }

        public void CreateBackup(string gameSaveFolder, string backupFolder, bool clearSourceFolder = false)
        {
            // Создать папку бэкапа, если нет
            if (!Directory.Exists(backupFolder))
                Directory.CreateDirectory(backupFolder);
            // Сделать бэкап текущих сохранений
            BackupFolder(gameSaveFolder, backupFolder);
            // удалить папку MyGames если был установлен флаг
            if (clearSourceFolder)
                ClearFolder(gameSaveFolder);
        }


        /// <summary>
        /// Сохранение папки сохранений предыдущих проектов 
        /// </summary>
        /// <param name="lastProjects">Последние запущенные проекты</param>
        /// <param name="currentGame">Текущий запускаемый проект</param>
        public void BackupPreviousProjects(List<ProjectsState> lastProjects,
            GameButtonModel currentGame)
        {
            // экстра бекап
            if (lastProjects == null || !lastProjects.Any())
            {
                string gameSaveFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), currentGame.GameSaveFolder);
                string extraBackupFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LizeriumExtraBackup");
                string launcherRootPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string backupFolderPath = Path.Combine(launcherRootPath, currentGame.BackupFolder, currentGame.NameMode);

                // бекапим и очищаем папку MyGames (папку с вероятно левым сохранением)
                BackupUncnownSaveData(gameSaveFolderPath, extraBackupFolder);
                _logger.LogInformation("Save extra backup to " + extraBackupFolder);
                return;
            }

            //var handledFolders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            //foreach (var proj in lastProjects)
            //{
            //    // Пропускаем текущий проект
            //    if (proj.LastActiveProject == currentGame.NameMode)
            //        continue;

            //    // Ищем модель проекта в настройках
            //    var item = _appSettings.GameButtonInfo.FirstOrDefault(it => it.NameMode == proj.LastActiveProject);
            //    if (item == null)
            //        continue;

            //    // Проверяем, не делали ли уже бэкап для этой папки
            //    if (handledFolders.Contains(item.GameSaveFolder))
            //        continue;

            //    handledFolders.Add(item.GameSaveFolder);

            //    var myDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //    string gameSaveFolderPath = Path.Combine(myDocs, item.GameSaveFolder);
            //    string backupFolderPath = Path.Combine(myDocs, item.BackupFolder, proj.LastActiveProject);

            //    // Создаем бэкап без удаления исходной папки, чтобы не терять данные
            //    CreateBackup(gameSaveFolderPath, backupFolderPath, clearSourceFolder: true);
            //}
        }

        public List<ProjectsState> GetLastNameStartProject()
        {
            var info = _projectActivityMonitor.GetLastActiveProject();
            if (info != null)
            {
                return info;
            }
            else return new List<ProjectsState>();
        }

        public void LoadSaveGame(string backupFolder, string gameSaveFolder)
        {
            try
            {
                _appSettings.CopyFolder(backupFolder, gameSaveFolder);

                _logger.LogInformation($"[GameSaveManagerService] {backupFolder}   [->]   {gameSaveFolder} - загружено");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[GameSaveManagerService] {backupFolder}   [->]    {gameSaveFolder} - не загружено [{ex.Message}]");
            }
        }

        /// <summary>
        /// Бэкап - копировать на время в папку бэкапа из sourceDir
        /// </summary>
        /// <param name="sourceDir">Откуда</param>
        /// <param name="backupDir">Куда</param>
        private void BackupFolder(string sourceDir, string backupDir, bool clearBackup = true)
        {
            try
            {
                // Можно очистить бэкап перед копированием
                if (clearBackup)
                    ClearFolder(backupDir);
                // копирование папки с вложениями в бекап директорию
                _appSettings.CopyFolder(sourceDir, backupDir);

                _logger.LogInformation($"[GameSaveManagerService] {sourceDir}   [->]    {backupDir} [clear::{clearBackup}] - бекап завершён!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[GameSaveManagerService] {sourceDir}   [->]    {backupDir} [clear::{clearBackup}] - бекап не получился [{ex.Message}]");
            }
        }

        /// <summary>
        /// Очистка папки от всех файлов и папок
        /// </summary>
        /// <param name="folderPath">Директория</param>
        private void ClearFolder(string folderPath)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    _logger.LogWarning($"[GameSaveManagerService] {folderPath} - не существует директории для удаления!");
                    return;
                }

                var dirInfo = new DirectoryInfo(folderPath);

                foreach (var file in dirInfo.GetFiles())
                    file.Delete();

                foreach (var dir in dirInfo.GetDirectories())
                    dir.Delete(true);

                _logger.LogInformation($"[GameSaveManagerService] {folderPath} - удалён!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[GameSaveManagerService] {folderPath}  - не удалён [{ex.Message}]");
            }
        }
    }
}
