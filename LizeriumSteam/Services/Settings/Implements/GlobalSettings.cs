/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 07 апреля 2026 11:13:20
 * Version: 1.0.12
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Linq;

using LizeriumSteam.Models.Games;
using LizeriumSteam.Models.Settings;
using LizeriumSteam.Views;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace LizeriumSteam.Services.Settings.Implements
{
    public sealed class GlobalSettings : IAppSettings
    {
        public AppGlobalSettings AppGlobalSettings { get; private set; } = new AppGlobalSettings();
        public List<GameButtonModel> GameButtonInfo { get; set; } = new List<GameButtonModel>();

        public event Action SettingsLoaded;

        private readonly string _settingsPath;
        private string _сonfigAppUpdaterPath 
            => Path.Combine(Path.GetDirectoryName(typeof(MainView).Assembly.Location), "..", "config.xml");

        private ILogger<GlobalSettings> _logger;

        public GlobalSettings(string SettingsPath, ILogger<GlobalSettings> logger)
        {
            _settingsPath = SettingsPath;
            _logger = logger;
        }

        public void Load()
        {
            try
            {
                if (!File.Exists(_settingsPath))
                    throw new FileNotFoundException($"Файл настроек не найден: {_settingsPath}");
                LoadGlobalConfiguration();
                LoadPanelsConfiguration();
                SettingsLoaded.Invoke();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} | {ex.StackTrace}");
            }
        }

        private void LoadGlobalConfiguration()
        {
            try
            {
                var json = File.ReadAllText(_settingsPath);
                AppGlobalSettings = JsonSerializer.Deserialize<AppGlobalSettings>(json) ?? new AppGlobalSettings();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при загрузке настроек: {_settingsPath} |{ex.Message} | {ex.StackTrace}");
            }
        }


        /// <summary>
        /// Сохраняет текущие настройки обратно в файл
        /// </summary>
        public void Save()
        {
            if (!File.Exists(_settingsPath))
                throw new FileNotFoundException($"Файл настроек не найден: {_settingsPath}");

            var json = JsonSerializer.Serialize(AppGlobalSettings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_settingsPath, json);
        }

        /// <summary>
        /// Загружает XML конфигурацию настроек панелей загрузчика
        /// </summary>
        public void LoadPanelsConfiguration()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(_сonfigAppUpdaterPath);

            var xdoc = XDocument.Load(_сonfigAppUpdaterPath);

            if (xdoc == null)
            {
                _logger.LogWarning("Раздел 'mods' не найден в XML: {Path}", _сonfigAppUpdaterPath);
                return;
            }

            try
            {
                GameButtonInfo.Clear();
                AppGlobalSettings.Version = xdoc.Root?.Element("version").Value;
                var mods = xdoc.Root?.Element("mods")?.Elements() ?? Enumerable.Empty<XElement>();
                foreach (var mod in mods)
                {
                    var project = new GameButtonModel
                    {
                        NameMode = mod.Name.LocalName,
                        Title = new LocalizedString()
                        {
                            en = (string)mod.Element("Title")?.Element("En") ?? "",
                            ru = (string)mod.Element("Title")?.Element("Ru") ?? "",
                        },
                        ServerSettings = new ServerSettingsModel()
                        {
                            Ip = (string)mod.Element("Server")?.Element("Ip") ?? "",
                            Port = (string)mod.Element("Server")?.Element("Port") ?? "",
                        },
                        GameFolderFullPath = (string)mod.Element("Folder"),
                        Updated = (bool?)mod.Element("Updated") ?? false,
                        StartupFolder = (string)mod.Element("StartupFolder"),
                        SupportUrl = (string)mod.Element("SupportUrl"),
                        GameSaveFolder = (string)mod.Element("GameSaveFolder"),
                        BackupFolder = (string)mod.Element("BackupFolder"),
                        StartupFile = (string)mod.Element("StartupFile"),
                        NameFileInstaller = (string)mod.Element("Installer"),
                        LastVersionInstall = (string)mod.Element("LastVersionInstall")
                    };
                    GameButtonInfo.Add(project);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при загрузке настроек: {_сonfigAppUpdaterPath} | {ex.Message} | {ex.StackTrace}");
            }
        }

        public void ChangePanelsConfigurationKey(string key, string newKey)
        {
            ChangeXMLValueKey(_сonfigAppUpdaterPath, key, newKey);
        }

        private string ChangeXMLValueKey(string xmlFilePath, string key, string newKey)
        {
            // Загрузите XML-документ
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);

            // Найдите узел freelancerPath
            XmlNode freelancerPathNode = doc.SelectSingleNode($"//config/{key}");

            if (freelancerPathNode != null)
            {
                // Измените значение узла
                freelancerPathNode.InnerText = newKey;

                // Сохраните изменения
                doc.Save(xmlFilePath);

                return $"{key} успешно изменен на: {newKey}";
            }
            else
            {
                return $"Узел {key} не найден.";
            }
        }

        /// <summary>
        /// Копирование папки с вложениями
        /// </summary>
        /// <param name="sourceDir">Откуда</param>
        /// <param name="targetDir">Куда</param>
        public void CopyFolder(string sourceDir, string targetDir)
        { 
            try
            {
                if (!Directory.Exists(sourceDir))
                {
                    return;
                }
                foreach (var dirPath in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(dirPath.Replace(sourceDir, targetDir));
                }

                foreach (var filePath in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
                {
                    File.Copy(filePath, filePath.Replace(sourceDir, targetDir), true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при копирование папки с вложениями: {ex.Message} | {ex.StackTrace}");
            }
        }
    }
}
