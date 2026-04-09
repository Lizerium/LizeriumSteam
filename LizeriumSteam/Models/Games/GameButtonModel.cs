/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 09 апреля 2026 11:13:54
 * Version: 1.0.14
 */

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;

using LizeriumSteam.Models.Settings;

namespace LizeriumSteam.Models.Games
{
    public enum DownloadState
    {
        Hidden,
        Visible
    }

    public enum StateMode
    {
        NotInstall,
        InstallNow,
        ErrorSrv,
        Outdated,
        Install,
        InGame
    }

    public enum LangugesEnum
    {
        [Description("English")]
        en,
        [Description("Русский")]
        ru
    }

    public class CheckLanguagesState
    {
        public bool IsExistLangs { get; set; }
        public string PathFolderConfig { get; set; }
        public string PathToFileConfig { get; set; }

        public LangugesEnum Language { get; set; }
        public List<LangugesEnum> AvailableLanguages { get; set; } = new List<LangugesEnum>();
    }

    public class GameButtonModel
    {
        public GameButtonModel()
        {
            ErrorServer = false;
        }

        public LocalizedString Title { get; set; }
        public string CurrentLanguageView { get; set; }
        public string TitleView => (CurrentLanguageView == "ru") ? Title.ru : Title.en;
        /// <summary>
        /// Имя продукта
        /// </summary>
        public string NameMode { get; set; }
        /// <summary>
        /// Обновляемый ли продукт
        /// </summary>
        public bool Updated { get; set; }
        /// <summary>
        /// Есть ли у продукта переводы на другие языки
        /// </summary>
        public CheckLanguagesState LanguagesState { get; set; }
        /// <summary>
        /// Путь до папки установки мода с его именем включительно
        /// </summary>
        public string GameFolderFullPath { get; set; }
        /// <summary>
        /// Имя нода в XML конфиге с указанием имени установщика внутри архива
        /// </summary>
        public string NameModeInstaller { get; set; }
        /// <summary>
        /// Имя установщика внутри архива скачанного с сервера
        /// </summary>
        public string NameFileInstaller { get; set; }
        /// <summary>
        /// Последняя установленная версия
        /// </summary>
        public string LastVersionInstall { get; set; }
        /// <summary>
        /// Имя установщика внутри архива скачанного с сервера без расширения
        /// </summary>
        public string NameFileInstallerNotExt => Path.ChangeExtension(NameFileInstaller, null);
        /// <summary>
        /// Версия которая установлена на текущий момент
        /// </summary>
        public string GameInstallVersion { get; set; }
        /// <summary>
        /// Статус приложения в приложении 
        /// </summary>
        public StateMode StateMode { get; set; }

        /// <summary>
        /// Статус приложения в приложении 
        /// </summary>
        public bool ErrorServer { get; set; }

        /// <summary>
        /// Директория в которой лежит файл запуска
        /// </summary>
        public string StartupFolder { get; set; }

        /// <summary>
        /// Директория сохранений игры
        /// </summary>
        public string GameSaveFolder { get; set; }

        /// <summary>
        /// Директория бекапов сохранений
        /// </summary>
        public string BackupFolder { get; set; }

        /// <summary>
        /// Файл запуска самого приложения
        /// </summary>
        public string StartupFile { get; set; }

        /// <summary>
        /// Версия до которой нужно обновиться
        /// </summary>
        public string NewVersionToServer { get; set; }

        /// <summary>
        /// Путь до файла с обновлениями
        /// </summary>
        public string FileInfoUpdatesPath => Path.Combine(GameFolderFullPath, "updates_info.json");

        /// <summary>
        /// Адрес до изображения Preiew ячейки
        /// </summary>
        public List<GameImageModel> ImageSource
        {
            get
            {
                var list = new List<GameImageModel>();
                // Генерируем три превью
                for (int i = 1; i <= 4; i++)
                {
                    list.Add(new GameImageModel()
                    {
                        // Формируем путь, например: GameModePreview1.jpg, GameModePreview2.jpg, GameModePreview3.jpg
                        ImageSource = $"pack://application:,,,/Resources/ButtonGame/{NameMode}Preview{i}.jpg",
                        OverlayText = TitleView,
                        StatusText = (StateMode == StateMode.NotInstall) ? "Не установлен" : ""
                    });
                }
                return list;
            }
        }

        /// <summary>
        /// URL-адрес страницы поддержки продукта
        /// </summary>
        public string SupportUrl { get; set; }

        /// <summary>
        /// Есть ли список с историей изменений
        /// </summary>
        public bool AvailableChangesHistory { get; set; }

        /// <summary>
        /// Настройки сервера
        /// </summary>
        public ServerSettingsModel ServerSettings { get; set; }
        public bool ExistFileInfoUpdatesPath { get; internal set; }

        /// <summary>
        /// Новая версия
        /// </summary>
        public string NewVersion;
    }
}
