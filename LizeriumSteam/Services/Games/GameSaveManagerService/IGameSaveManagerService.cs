/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 06 мая 2026 10:50:54
 * Version: 1.0.43
 */

using System.Collections.Generic;

using LizeriumSteam.Models.Games;
using LizeriumSteam.Services.Games.ProjectMonitorService.Components;

namespace LizeriumSteam.Services.Games.GameSaveManagerService
{
    public interface IGameSaveManagerService
    {
        /// <summary>
        /// Получить имя последнего запущенного проекта
        /// </summary>
        List<ProjectsState> GetLastNameStartProject();

        /// <summary>
        /// Создание бэкапа сохранения игры
        /// </summary>
        /// <param name="gameSaveFolder">Папка с сохранением игры</param>
        /// <param name="backupFolder">Папка для хранения сохранения игры</param>
        /// <param name="clearSourceFolder">Удалять ли папку gameSaveFolder</param>
        void CreateBackup(string gameSaveFolder, string backupFolder, bool clearSourceFolder = false);

        /// <summary>
        /// Загрузка сохранения игры в папку (например, Documents)
        /// </summary>
        /// <param name="backupFolder">Папка с сохранением</param>
        /// <param name="gameSaveFolder">Целевая папка игры</param>
        void LoadSaveGame(string backupFolder, string gameSaveFolder);

        /// <summary>
        /// Сохранение данных от неизвестного проекта
        /// </summary>
        /// <param name="gameSaveFolder">Источник данных</param>
        /// <param name="extraBackupFolder">Куда сохранить</param>
        void BackupUncnownSaveData(string gameSaveFolder, string extraBackupFolder);

        /// <summary>
        /// Сохранение папки сохранений предыдущих проектов 
        /// </summary>
        /// <param name="lastProjects">Последние запущенные проекты</param>
        /// <param name="currentGame">Текущий запускаемый проект</param>
        void BackupPreviousProjects(List<ProjectsState> lastProjects,
            GameButtonModel currentGame);
    }
}
