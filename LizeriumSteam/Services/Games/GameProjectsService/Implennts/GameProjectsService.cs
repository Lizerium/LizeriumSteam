/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 18 апреля 2026 15:00:38
 * Version: 1.0.25
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;

using AppUpdater;

using LizeriumSteam.Models.Games;
using LizeriumSteam.Services.Games.GameProjectsService;
using LizeriumSteam.Services.Games.GameProjectsService.Components;
using LizeriumSteam.Services.Settings;

using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace LizeriumSteam.Services.Games.GameProjectsService.Implennts
{
    /// <summary>
    /// Cервис загрузки и обновления информации о проектах
    /// </summary>
    public class GameProjectsService : IProjectsService
    {
        public ObservableCollection<GameButtonModel> GameButtons { get; set; } = new ObservableCollection<GameButtonModel>();

        private readonly ILogger<GameProjectsService> _logger;
        private readonly IAppSettings _appSettings;

        public GameProjectsService(ILogger<GameProjectsService> logger,
            IAppSettings appSettings) 
        { 
            _logger = logger;
            _appSettings = appSettings;
        }

        /// <summary>
        /// Инициализация проектов
        /// </summary>
        public async Task Initialize()
        {
            foreach(var game in _appSettings.GameButtonInfo)
            {
                game.StateMode = await CheckAndSetupStatesProject(game);
                GameButtons.Add(game);
            }
        }

        /// <summary>
        /// Провка и фиксация состояний проекта
        /// </summary>
        /// <param name="data">Модель данных проекта</param>
        /// <returns>StateMode</returns>
        public async Task<StateMode> CheckAndSetupStatesProject(GameButtonModel data)
        {
            // проверяем существование Freelancer
            if (string.IsNullOrEmpty(data.NameMode)) return StateMode.NotInstall;
            // проверяем существование мода
            var isMode = await IsModDirectory(data);
            if (isMode == StateMode.Install)
            {
                var langInfo = GetCurrentTranslate(data);
                data.LanguagesState = langInfo;
            }
            return isMode;
        }

        /// <summary>
        /// Проверка наличия проектов и их обновлений
        /// </summary>
        /// <param name="data">Модель данных проекта</param>
        /// <returns>StateMode</returns>
        private async Task<StateMode> IsModDirectory(GameButtonModel data)
        {
            // есть ли продукт у пользователя
            // если есть папка и существует установочный файл то проект есть
            // если файла нет проверяем реестр на наличие пути
            if (!ProjectIsExist(data.GameFolderFullPath, data.StartupFolder, data.StartupFile))
            {
                var check = CheckProjectRegistry(data.NameMode);
                if (!check.IsExist)
                {
                    data.StateMode = StateMode.NotInstall;
                    return data.StateMode;
                }
                else
                {
                    _appSettings.ChangePanelsConfigurationKey($"mods/{data.NameMode}/Folder", check.path);
                    data.GameFolderFullPath = check.path;
                }
            }

            // запущен ли процесс игры
            var processStartupInGame = Process.GetProcessesByName(Path.ChangeExtension(data.StartupFile, null));
            if (processStartupInGame != null && processStartupInGame.Length > 0)
            {
                data.StateMode = StateMode.InGame;
                return data.StateMode;
            }

            // если проект не обновляется достаточно его наличие
            if (!data.Updated)
            {
                data.StateMode = StateMode.Install;
                return StateMode.Install;
            }

            // проверяю наличие манифеста обновления
            var pathGameConfig = Path.Combine(data.GameFolderFullPath, "manifest.launcher");
            if (!File.Exists(pathGameConfig))
            {
                data.StateMode = StateMode.NotInstall;
                return data.StateMode;
            }

            // проверяем статус запуска установщика если он запущен для игры
            var process = Process.GetProcessesByName(data.NameFileInstallerNotExt);
            if (process != null && process.Length > 0)
            {
                data.StateMode = StateMode.InstallNow;
                return data.StateMode;
            }

            try
            {
                // проверяю известную лаунчеру версию проекта
                var xdoc = XDocument.Load(pathGameConfig);
                if (xdoc == null)
                {
                    data.StateMode = StateMode.NotInstall;
                    return data.StateMode;
                }

                var versionProject = xdoc.Root?.Element("version").Value;
                if (string.IsNullOrEmpty(versionProject))
                {
                    data.StateMode = StateMode.NotInstall;
                    return data.StateMode;
                }
                data.GameInstallVersion = versionProject;
                data.StateMode = StateMode.Install;

                //проверяем устаревание
                var updates = await UpdateManager.Default.UpdateServer.GetCurrentUpdateModeAsync(data.NameMode);
                if (string.IsNullOrEmpty(updates))
                    data.ErrorServer = true;
                else data.ErrorServer = false;

                if (!updates.Contains(versionProject) && !data.ErrorServer)
                {
                    data.StateMode = StateMode.Outdated;
                    data.NewVersion = $"Доступна 🔁 {updates}";
                    data.NewVersionToServer = updates;
                    return data.StateMode;
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, $"Error check states projects...");
            }

            return StateMode.Install;
        }

        /// <summary>
        /// Проверка существования папки с игрой и файла запуска игры
        /// </summary>
        /// <param name="folder">Папка с игрой (path/to/GameFolder)</param>
        /// <param name="startupFolder">Папка в которой файл запуска (EXE)</param>
        /// <param name="startupFile">Название файла запуска игры (game.exe)</param>
        /// <returns></returns>
        private bool ProjectIsExist(string folder, string startupFolder, string startupFile)
        {
            try
            {
                bool state1 = Directory.Exists(folder);
                var pathToStartupFile = Path.Combine(folder, startupFolder, startupFile);
                bool state2 = File.Exists(pathToStartupFile);
                return state1 && state2;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error parse folder project... {ex.Message} | {ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Проверка зарегистрированного пути установки проекта
        /// </summary>
        /// <param name="nameProject">Имя проекта</param>
        /// <returns>RegistryResponse</returns>
        private RegistryResponse CheckProjectRegistry(string nameProject)
        {
            string path = Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + nameProject, "InstallPath", null) as string;

            if (!string.IsNullOrEmpty(path))
            {
                return new RegistryResponse()
                {
                    IsExist = true,
                    nameProject = nameProject,
                    path = path
                };
            }
            else
            {
                return new RegistryResponse()
                {
                    IsExist = false,
                    nameProject = nameProject,
                    path = string.Empty
                };
            }
        }

        /// <summary>
        /// Проверка наличия переводов в игре
        /// </summary>
        /// <param name="data">Модель данных проекта</param>
        /// <returns>CheckLanguagesState</returns>
        private CheckLanguagesState GetCurrentTranslate(GameButtonModel data)
        {
            var pathTranslateConfigDir = Path.Combine(data.GameFolderFullPath, "TRANSLATES");
            var pathTranslateConfigFile = string.Empty;
            var available = new List<LangugesEnum>();

            if (Directory.Exists(pathTranslateConfigDir))
            {
                // собираем имена папок (EN, RU, ...)
                foreach (var dir in Directory.GetDirectories(pathTranslateConfigDir))
                {
                    var name = Path.GetFileName(dir);
                    if (string.IsNullOrWhiteSpace(name))
                        continue;

                    if (Enum.TryParse<LangugesEnum>(name, ignoreCase: true, out var parsedLang))
                    {
                        if (!available.Contains(parsedLang))
                            available.Add(parsedLang);
                    }
                }

                pathTranslateConfigFile = Path.Combine(pathTranslateConfigDir, "translate_info.json");
                if (File.Exists(pathTranslateConfigFile))
                {
                    var dataFile = File.ReadAllText(pathTranslateConfigFile);
                    var config = JsonSerializer.Deserialize<GameLanguageConfig>(dataFile);
                    return new CheckLanguagesState()
                    {
                        PathFolderConfig = pathTranslateConfigDir,
                        PathToFileConfig = pathTranslateConfigFile,
                        IsExistLangs = true,
                        AvailableLanguages = available,
                        Language = (LangugesEnum)Enum.Parse(typeof(LangugesEnum), config.CurrentTranslate)
                    };
                }
            }

            return new CheckLanguagesState()
            {
                PathFolderConfig = pathTranslateConfigDir,
                PathToFileConfig = pathTranslateConfigFile,
                IsExistLangs = false,
                AvailableLanguages = available,
                Language = LangugesEnum.en
            };
        }
    }
}
