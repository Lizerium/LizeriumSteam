/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 16 апреля 2026 11:59:53
 * Version: 1.0.23
 */

using System;
using System.Collections.Generic;

using LizeriumSteam.Models.Games;
using LizeriumSteam.Models.Settings;

namespace LizeriumSteam.Services.Settings
{
    public interface IAppSettings
    {
        event Action SettingsLoaded;

        AppGlobalSettings AppGlobalSettings { get; }
        List<GameButtonModel> GameButtonInfo { get; }

        void Load();
        void Save();

        /// <summary>
        /// Копирование папки с вложениями
        /// </summary>
        /// <param name="sourceDir">Откуда</param>
        /// <param name="targetDir">Куда</param>
        void CopyFolder(string sourceDir, string targetDir);
        void ChangePanelsConfigurationKey(string key, string newKey);
    }
}
