/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 12 апреля 2026 14:31:32
 * Version: 1.0.17
 */

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using HandyControl.Controls;
using HandyControl.Data;

using LizeriumSteam.Events;
using LizeriumSteam.Events.Unpack;
using LizeriumSteam.Models.Games;
using LizeriumSteam.Models.Games.GameUpdate;
using LizeriumSteam.Services.Games.GameProjectsService;
using LizeriumSteam.Services.Games.GameProjectsService.Components;
using LizeriumSteam.Services.Games.GameSaveManagerService;
using LizeriumSteam.Services.Games.GameUpdateService;
using LizeriumSteam.Services.Lang.Implements;
using LizeriumSteam.Services.Settings;
using LizeriumSteam.Services.Tray.Windows;

using Microsoft.Extensions.Logging;

using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace LizeriumSteam.ViewModels
{
    public class GameButtonViewModel : BindableBase
    {
        private readonly ILogger<GameButtonViewModel> _logger;
        private readonly IGameSaveManagerService _gameSaveManagerService;
        private readonly IProjectsService _projectsService;
        private readonly IAppSettings _appSetings;
        private readonly IEventAggregator _eventAggregator;
        private readonly ITrayAlertsService _trayAlertsService;
        private IProjectUpdateService _projectUpdateService;
        private readonly DispatcherTimer _timer;

        private GameButtonModel _gameButtonModel {  get; set; }
        private string currentDownloadState = "norm";
        private bool IsFirstDownloadInstallerStarted = false;
        private bool IsBackgroundUpdateUI { get; set; } = true;

        #region props

        #region Update Block

        private string _newVersion;
        public string NewVersion
        {
            get
            {
                _newVersion = _gameButtonModel.NewVersion;
                return _newVersion;
            }
            set => SetProperty(ref _newVersion, value);
        }

        public string _lastVersion;
        public string LastVersion
        {
            get
            {
                _lastVersion = _gameButtonModel.LastVersionInstall;
                return _lastVersion;
            }
            set => SetProperty(ref _lastVersion, value);
        }

        public string _newVersionToServer;
        public string NewVersionToServer
        {
            get
            {
                _newVersionToServer = _gameButtonModel.NewVersionToServer;
                return _newVersionToServer;
            }
            set => SetProperty(ref _newVersionToServer, value);
        }

        public string VisibleLabelState { get; set; }

        private string _labelUpdateText = "Нажмите для загрузки 🚇";
        public string LabelUpdateText
        {
            get
            {
                return _labelUpdateText;
            }
            set => SetProperty(ref _labelUpdateText, value);
        }

        public double _progressUpdateValue = 0;
        public double ProgressUpdateValue
        {
            get
            {
                return _progressUpdateValue;
            }
            set => SetProperty(ref _progressUpdateValue, value);
        }

        public Visibility _visibleDownload = Visibility.Hidden;
        public Visibility VisibleDownload
        {
            get
            {
                return _visibleDownload;
            }
            set => SetProperty(ref _visibleDownload, value);
        }

        public Visibility _visibleBtnDownload = Visibility.Visible;
        public Visibility VisibleBtnDownload
        {
            get
            {
                return _visibleBtnDownload;
            }
            set => SetProperty(ref _visibleBtnDownload, value);
        }

        public string _needUpdateStateText = (Application.Current.TryFindResource("NeedUpdateStateText") as string
                            ?? $"Доступно обновление!");
        public string NeedUpdateStateText
        {
            get
            {
                return _needUpdateStateText;
            }
            set => SetProperty(ref _needUpdateStateText, value);
        }

        private int _fileDownloadQueue = 0;

        public int FileDownloadQueue
        {
            get { return _fileDownloadQueue; }
            set => SetProperty(ref _fileDownloadQueue, value); 
        }

        private int _fileDownloadAll = 1;

        public int FileDownloadAll
        {
            get { return _fileDownloadAll; }
            set => SetProperty(ref _fileDownloadAll, value);
        }

        private Visibility _isUnpackFiles = Visibility.Collapsed;

        public Visibility IsUnpackFiles
        {
            get { return _isUnpackFiles; }
            set => SetProperty(ref _isUnpackFiles, value);
        }

        private int _fileUnpackQueue = 0;

        public int FileUnpackQueue
        {
            get { return _fileUnpackQueue; }
            set => SetProperty(ref _fileUnpackQueue, value);
        }

        private Visibility _isDeleteFiles = Visibility.Collapsed;

        public Visibility IsDeleteFiles
        {
            get { return _isDeleteFiles; }
            set => SetProperty(ref _isDeleteFiles, value);
        }

        private int _filesDeleteQueue = 0;

        public int FileDeleteQueue
        {
            get { return _filesDeleteQueue; }
            set => SetProperty(ref _filesDeleteQueue, value);
        }

        private Visibility _isDeleteDirs = Visibility.Collapsed;

        public Visibility IsDeleteDirs
        {
            get { return _isDeleteDirs; }
            set => SetProperty(ref _isDeleteDirs, value);
        }


        private int _dirsDeleteQueue = 0;

        public int DirsDeleteQueue
        {
            get { return _dirsDeleteQueue; }
            set => SetProperty(ref _dirsDeleteQueue, value);
        }

        private Visibility _isDownloadFirstProcess = Visibility.Visible;

        public Visibility IsDownloadFirstProcess
        {
            get { return _isDownloadFirstProcess; }
            set => SetProperty(ref _isDownloadFirstProcess, value);
        }

        #endregion

        public string TitleView => _gameButtonModel.TitleView;

        private StateMode _stateMode;
        public StateMode StateMode
        {
            get => _stateMode;
            set
            {
                SetProperty(ref _stateMode, value);
                RaisePropertyChanged(nameof(InGameState));
                RaisePropertyChanged(nameof(InGameStateMenu));
                RaisePropertyChanged(nameof(StartGameState));
                RaisePropertyChanged(nameof(MenuStartGameState));
                RaisePropertyChanged(nameof(DownloadState));
                RaisePropertyChanged(nameof(DownloadStateMenu));
                RaisePropertyChanged(nameof(IsExistGameState));
                RaisePropertyChanged(nameof(UpdateState));
                RaisePropertyChanged(nameof(UpdateStateMenu));
                RaisePropertyChanged(nameof(IsMiniVisibleBtnDownload));
                RaisePropertyChanged(nameof(AvailableLeftMenu));
            }
        }


        private Visibility _availableLangs = Visibility.Collapsed;

        public Visibility AvailableLangs
        {
            get => _availableLangs;
            set => SetProperty(ref _availableLangs, value);
        }

        private Visibility _availableListChanges = Visibility.Collapsed;

        public Visibility AvailableListChanges
        {
            get => _availableListChanges;
            set => SetProperty(ref _availableListChanges, value);
        }

        private Visibility _availableLeftMenu = Visibility.Collapsed;

        public Visibility AvailableLeftMenu
        {
            get 
            { 
                if(AvailableLangs == Visibility.Visible
                    || AvailableListChanges == Visibility.Visible)
                    return Visibility.Visible;
                return _availableLeftMenu; 
            }
            set => SetProperty(ref _availableLeftMenu, value);
        }

        private LangugesEnum _selectedLanguage;
        public LangugesEnum SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                if (_selectedLanguage != value)
                {
                    _selectedLanguage = value;
                    RaisePropertyChanged(nameof(SelectedLanguage)); // Prism
                }
            }
        }

        public string InGameState
        {
            get
            {
                switch (_gameButtonModel.StateMode)
                {
                    case StateMode.NotInstall:
                    case StateMode.InstallNow:
                    case StateMode.Outdated:
                    case StateMode.Install:
                        return Visibility.Hidden.ToString();
                    case StateMode.InGame:
                        return Visibility.Visible.ToString();
                    default:
                        return Visibility.Hidden.ToString();
                }
            }
        }

        public bool InGameStateMenu
        {
            get
            {
                switch (_gameButtonModel.StateMode)
                {
                    case StateMode.NotInstall:
                    case StateMode.InstallNow:
                    case StateMode.Outdated:
                    case StateMode.Install:
                        return false;
                    case StateMode.InGame:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public string StartGameState
        {
            get
            {
                switch (_gameButtonModel.StateMode)
                {
                    case StateMode.Install:
                        return Visibility.Visible.ToString();
                    case StateMode.NotInstall:
                    case StateMode.InstallNow:
                    case StateMode.Outdated:
                    case StateMode.InGame:
                        return Visibility.Hidden.ToString();
                    default:
                        return Visibility.Hidden.ToString();
                }
            }
        }

        public bool MenuStartGameState
        {
            get
            {
                switch (_gameButtonModel.StateMode)
                {
                    case StateMode.Install:
                        return true;
                    case StateMode.NotInstall:
                    case StateMode.InstallNow:
                    case StateMode.Outdated:
                    case StateMode.InGame:
                        return false;
                    default:
                        return false;
                }
            }
        }

        public string DownloadState
        {
            get
            {
                switch (_gameButtonModel.StateMode)
                {
                    case StateMode.NotInstall:
                    case StateMode.InstallNow:
                        return Visibility.Visible.ToString();
                    case StateMode.Outdated:
                    case StateMode.Install:
                    case StateMode.InGame:
                        return Visibility.Hidden.ToString();
                    default:
                        return Visibility.Visible.ToString();
                }
            }
        }

        public bool DownloadStateMenu
        {
            get
            {
                switch (_gameButtonModel.StateMode)
                {
                    case StateMode.NotInstall:
                    case StateMode.InstallNow:
                        if(IsFirstDownloadInstallerStarted)
                            return false;
                        return true;
                    case StateMode.Outdated:
                    case StateMode.Install:
                    case StateMode.InGame:
                        return false;
                    default:
                        return true;
                }
            }
        }

        public string IsExistGameState
        {
            get
            {
                switch (_gameButtonModel.StateMode)
                {
                    case StateMode.Install:
                        return Visibility.Visible.ToString();
                    case StateMode.Outdated:
                    case StateMode.NotInstall:
                    case StateMode.InstallNow:
                    case StateMode.InGame:
                        return Visibility.Hidden.ToString();
                    default:
                        return Visibility.Hidden.ToString();
                }
            }
        }

        public Visibility UpdateState
        {
            get
            {
                switch (_gameButtonModel.StateMode)
                {
                    case StateMode.NotInstall:
                        if (IsFirstDownloadInstallerStarted)
                            return Visibility.Visible;
                        else
                            return Visibility.Hidden;
                        break;
                    case StateMode.InstallNow:
                    case StateMode.Install:
                    case StateMode.InGame:
                        return Visibility.Hidden;
                    case StateMode.Outdated:
                        
                        return Visibility.Visible;
                    default:
                        return Visibility.Hidden;
                }
            }
        }

        public bool UpdateStateMenu
        {
            get
            {
                switch (_gameButtonModel.StateMode)
                {
                    case StateMode.NotInstall:
                    case StateMode.InstallNow:
                    case StateMode.Install:
                    case StateMode.InGame:
                        return false;
                    case StateMode.Outdated:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public Visibility IsMiniVisibleBtnDownload
        {
            get
            {
                switch (_gameButtonModel.StateMode)
                {
                    case StateMode.NotInstall:
                    case StateMode.InstallNow:
                    case StateMode.Install:
                    case StateMode.InGame:
                        return Visibility.Hidden;
                    case StateMode.Outdated:
                        if (VisibleBtnDownload == Visibility.Visible)
                            return Visibility.Visible;
                        else
                            return  Visibility.Hidden;
                    default:
                        return Visibility.Hidden;
                }
            }
        }

        public string _isUpdatedGame;
        /// <summary>
        /// Существуют ли обновления на приложения
        /// </summary>
        public string IsUpdatedGame
        {
            get
            {
                if (_gameButtonModel.Updated)
                    return Visibility.Visible.ToString();
                return Visibility.Hidden.ToString();
            }
            set => _isUpdatedGame = value;
        }

        public string _isFileExistTrackUpdatesGame;

        /// <summary>
        /// Существуют ли обновления на приложение и список этих обновлений в папке
        /// </summary>
        public string IsFileExistTrackUpdatesGame
        {
            get
            {
                if (!IsBackgroundUpdateUI) return Visibility.Hidden.ToString();

                if (_gameButtonModel.Updated && _gameButtonModel.ExistFileInfoUpdatesPath)
                    return Visibility.Visible.ToString();
                return Visibility.Hidden.ToString();
            }
            set => _isFileExistTrackUpdatesGame = value;
        }

        private bool _isEnabled = true;

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set => SetProperty(ref _isEnabled, value);
        }

        public string ImageSourceImage1
        {
            get
            {
                return _gameButtonModel.ImageSource[0].ImageSource;
            }
        }
        public string ImageSourceImage2
        {
            get
            {
                return _gameButtonModel.ImageSource[1].ImageSource;
            }
        }
        public string ImageSourceImage3
        {
            get
            {
                return _gameButtonModel.ImageSource[2].ImageSource;
            }
        }
        public string ImageSourceImage4
        {
            get
            {
                return _gameButtonModel.ImageSource[3].ImageSource;
            }
        }

        private Visibility _isPanelVisible = Visibility.Hidden;
        public Visibility IsPanelVisible
        {
            get => _isPanelVisible;
            set => SetProperty(ref _isPanelVisible, value);
        }

        private bool _isLangMenuOpen;
        public bool IsLangMenuOpen
        {
            get => _isLangMenuOpen;
            set => SetProperty(ref _isLangMenuOpen, value);
        }

        #endregion

        // Команды
        public DelegateCommand InGameCommand { get; }
        public DelegateCommand InstallCommand { get; }
        public DelegateCommand UpdateCommand { get; }
        public DelegateCommand StopUpdateCommand { get; }
        public DelegateCommand OpenConfigCommand { get; }
        public DelegateCommand StartGameCommand { get; }
        public DelegateCommand ForceStartGameCommand { get; }
        public DelegateCommand<object> ChangeLangGameCommand { get; }
        public DelegateCommand OpenBtnsMenuCommand { get; }
        public DelegateCommand OpenProjectUpdatesCommand { get; }

        public ObservableCollection<LangugesEnum> AvailableLanguages { get; } = new ObservableCollection<LangugesEnum>();


        public GameButtonViewModel(GameButtonModel model,
            ILogger<GameButtonViewModel> logger,
            IProjectsService projectsService,
            IGameSaveManagerService gameSaveManagerService,
            IAppSettings appSetings,
            IEventAggregator eventAggregator,
            ITrayAlertsService trayAlertsService,
            IProjectUpdateService projectUpdateService)
        {
            _logger = logger;
            _gameSaveManagerService = gameSaveManagerService;
            _projectsService = projectsService;
            _appSetings = appSetings;
            _eventAggregator = eventAggregator;
            _trayAlertsService = trayAlertsService;
            _projectUpdateService = projectUpdateService;

            _gameButtonModel = model;

            CheckAvailableParamsToLeftMenu();

            #region Commands

            InGameCommand = new DelegateCommand(InGame);
            InstallCommand = new DelegateCommand(OnInstall);
            UpdateCommand = new DelegateCommand(OnUpdate);
            StopUpdateCommand = new DelegateCommand(StopUpdate);
            OpenConfigCommand = new DelegateCommand(OnOpenConfig);
            StartGameCommand = new DelegateCommand(StartGame);
            ForceStartGameCommand = new DelegateCommand(ForceStartGame);
            ChangeLangGameCommand = new DelegateCommand<object>((p =>
            {
                if (p is LangugesEnum lang)
                    ChangeGameLang(lang);
            }));
            OpenBtnsMenuCommand = new DelegateCommand(OpenBtnsMenu);
            OpenProjectUpdatesCommand = new DelegateCommand(OpenProjectUpdatesInfo);

            #endregion

            #region Events

            // начало прогресса установки проекта
            _eventAggregator.GetEvent<AppGameInstallStartEvent>()
                .Subscribe(StartInstallGame, ThreadOption.UIThread);

            // начало прогресса обновления проекта
            _eventAggregator.GetEvent<AppGameUpdateStartEvent>()
                .Subscribe(StartUpdateGame, ThreadOption.UIThread);

            // прогресс обновления проекта
            _eventAggregator.GetEvent<AppGameUpdateProcessEvent>()
                .Subscribe(ProgressUpdateGame, ThreadOption.UIThread);

            // прогресс обновления загруженных файлов
            _eventAggregator.GetEvent<AppGameUpdateFilesEvent>()
                .Subscribe(SetAdditionalInfoFilesUpdate, ThreadOption.UIThread);

            // окончание прогресса обновления проекта
            _eventAggregator.GetEvent<AppGameUpdateStopEvent>()
                .Subscribe(StopUpdateGame, ThreadOption.UIThread);

            // прогресс распаковки загруженных файлов
            _eventAggregator.GetEvent<AppGameUnpackFilesEvent>()
                .Subscribe(UnpackFilesGame, ThreadOption.UIThread);

            // Подписка на событие изменения языка
            _eventAggregator.GetEvent<LangChangedCompleteEvent>().Subscribe(OnLanguageChanged);

            #endregion

            // Таймер обновления состояний
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            _timer.Tick += async (_, __) => await UpdateStatesUIAsync();
            _timer.Start();
        }
      
        private void CheckAvailableParamsToLeftMenu()
        {
            if(_gameButtonModel.StateMode == StateMode.NotInstall)
            {
                AvailableLangs = Visibility.Collapsed;
                AvailableListChanges = Visibility.Collapsed;
                AvailableLanguages.Clear();
                return;
            }

            if(_gameButtonModel.AvailableChangesHistory)
                AvailableListChanges = Visibility.Visible;
            else AvailableListChanges = Visibility.Collapsed;

            if (_gameButtonModel.LanguagesState != null && _gameButtonModel.LanguagesState.AvailableLanguages != null)
            {
                AvailableLanguages.Clear();
                foreach (var lang in _gameButtonModel.LanguagesState.AvailableLanguages)
                    AvailableLanguages.Add(lang);
                AvailableLangs = AvailableLanguages.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
                SelectedLanguage = _gameButtonModel.LanguagesState.Language;
            }
        }

        private void OnLanguageChanged()
        {
            SetAvailableUpdateStates(currentDownloadState);
        }

        /// <summary>
        /// Обновление состояний элементов ячейки проекта
        /// </summary>
        /// <returns></returns>
        private async Task UpdateStatesUIAsync()
        {
            if (!IsBackgroundUpdateUI) return;
            _gameButtonModel.StateMode = await _projectsService.CheckAndSetupStatesProject(_gameButtonModel);
            NewVersionToServer = _gameButtonModel.NewVersionToServer;
            LastVersion = _gameButtonModel.LastVersionInstall;
            StateMode = _gameButtonModel.StateMode;
            if(!IsLangMenuOpen)
                CheckAvailableParamsToLeftMenu();
        }

        private void OpenBtnsMenu()
        {
            if (IsPanelVisible == Visibility.Hidden)
                IsPanelVisible = Visibility.Visible;
            else IsPanelVisible = Visibility.Hidden;
        }

        /// <summary>
        /// Принудительный старт проекта
        /// 
        /// P.s с завершением всех процессов с его именем
        /// </summary>
        private void ForceStartGame()
        {
            IsPanelVisible = Visibility.Hidden;
            
            try
            {
                var processName = Path.GetFileNameWithoutExtension(_gameButtonModel.StartupFile);
                if (string.IsNullOrEmpty(processName))
                {
                    _logger.LogError($"StartupFile для {_gameButtonModel.NameMode} пустой!");
                    return;
                }

                // Получаем все процессы с именем игры
                var runningProcesses = Process.GetProcessesByName(processName);

                foreach (var proc in runningProcesses)
                {
                    try
                    {
                        _logger.LogInformation($"Завершаем процесс {proc.ProcessName} (Id: {proc.Id})");
                        proc.Kill(); // убиваем процесс
                        proc.WaitForExit(5000); // ждём максимум 5 секунд
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Не удалось завершить процесс {proc.ProcessName} (Id: {proc.Id}): {ex.Message}");
                    }
                }

                // После завершения всех процессов — обычный запуск
                StartGame();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ERROR] ForceStartGame: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Логика запуска проекта
        /// </summary>
        private void StartGame()
        {
            IsPanelVisible = Visibility.Hidden;

            try
            {
                var processStartupInGame = Process.GetProcessesByName(Path.ChangeExtension(_gameButtonModel.StartupFile, null));
                if (processStartupInGame != null && processStartupInGame.Length > 0)
                    return;

                var StartupFolder = _gameButtonModel.StartupFolder;
                if (string.IsNullOrEmpty(StartupFolder))
                {
                    _logger.LogError("mods/" + _gameButtonModel.NameMode + "/StartupFolder key to config.xml is null!");
                    return;
                }

                var StartupFile = _gameButtonModel.StartupFile;

                if (string.IsNullOrEmpty(StartupFile))
                {
                    _logger.LogError("mods/" + _gameButtonModel.NameMode + "/StartupFile key to config.xml is null!");
                    return;
                }

                var gameSaveFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), _gameButtonModel.GameSaveFolder);
                var backupFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), _gameButtonModel.BackupFolder, _gameButtonModel.NameMode);

                // Сохраняем старые проекты, которые использовали ту же папку
                _gameSaveManagerService.BackupPreviousProjects(
                        _gameSaveManagerService.GetLastNameStartProject(),
                        _gameButtonModel
                    );

                //// загрузить сохранение запускаемой игры
                //_gameSaveManagerService.LoadSaveGame(backupFolderPath, gameSaveFolderPath);

                // старт приложения
                var startup = Path.Combine(_gameButtonModel.GameFolderFullPath, StartupFolder, StartupFile);

                string serverIp = _gameButtonModel.ServerSettings?.Ip;
                string serverPort = _gameButtonModel.ServerSettings?.Port;

                string serverFlag = null;

                // проверяем, что IP указан
                if (!string.IsNullOrWhiteSpace(serverIp))
                {
                    // если порт пустой, можно запустить только с IP
                    if (string.IsNullOrWhiteSpace(serverPort))
                        serverFlag = $"-s{serverIp}";
                    else
                        serverFlag = $"-s{serverIp}:{serverPort}";
                }

                var startInfo = new ProcessStartInfo
                {
                    FileName = startup,
                    Arguments = serverFlag ?? string.Empty, // если null → запускаем без аргументов
                    UseShellExecute = false,
                    WorkingDirectory = Path.Combine(_gameButtonModel.GameFolderFullPath, StartupFolder)
                };

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] Open game:" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// Логика развёртывания окна игры
        /// </summary>
        private void InGame()
        {
            IsPanelVisible = Visibility.Hidden;

            try
            {
                var processName = Path.ChangeExtension(_gameButtonModel.StartupFile, null);
                var processes = Process.GetProcessesByName(processName);

                if (processes.Length == 0)
                {
                    _logger.LogWarning($"Игра {_gameButtonModel.NameMode} не найдена среди запущенных процессов.");
                    return;
                }

                var process = processes[0]; // берём первый найденный процесс

                IntPtr hWnd = process.MainWindowHandle;
                if (hWnd == IntPtr.Zero)
                {
                    _logger.LogWarning("У процесса нет главного окна или оно ещё не создано.");
                    return;
                }

                // Если окно свернуто — разворачиваем
                ShowWindow(hWnd, SW_RESTORE);

                // Переводим фокус на окно
                SetForegroundWindow(hWnd);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ERROR] InGame focus: {ex.Message}\n{ex.Source}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Логика установки игры с нуля
        /// </summary>
        private void OnInstall()
        {
            if (IsBackgroundUpdateUI == false) return;

            IsPanelVisible = Visibility.Hidden;

            _projectUpdateService.InstallProject(_gameButtonModel);
        }

        /// <summary>
        /// Логика обновления 
        /// </summary>
        private void OnUpdate()
        {
            if (IsBackgroundUpdateUI == false) return;

            IsPanelVisible = Visibility.Hidden;

            // останавливаю сервис мониторинга манифестов на время установки
            _eventAggregator.GetEvent<StopManifestMonitorEvent>().Publish();
            _projectUpdateService.UpdateProject(_gameButtonModel);
        }

        /// <summary>
        /// Логика остановки обновления 
        /// </summary>
        private void StopUpdate()
        {
            IsPanelVisible = Visibility.Hidden;

            _projectUpdateService.StopUpdateProject(_gameButtonModel);
        }

        /// <summary>
        /// Логика открытия настроек TODO:
        /// </summary>
        private void OnOpenConfig()
        {
            IsPanelVisible = Visibility.Hidden;
        }

        private void OpenProjectUpdatesInfo()
        {
            _eventAggregator.GetEvent<OpenProjectUpdatesInfoEvent>().Publish(_gameButtonModel);
        }

        /// <summary>
        /// Логика смены языка игры
        /// </summary>
        /// <param name="selectedLang">Выбранный язык</param>
        private void ChangeGameLang(LangugesEnum selectedLang)
        {
            // Логика смены языка игры
            try
            {
                #region Texts Game
                // формируем адреса до директорий откуда и куда будет копирование текстовой информации игры
                var folderNewLang = Path.Combine(_gameButtonModel.LanguagesState.PathFolderConfig,
                    selectedLang.ToString().ToUpper());
                var folderExe = Path.Combine(_gameButtonModel.GameFolderFullPath, _gameButtonModel.StartupFolder);
                if (!Directory.Exists(folderNewLang) || !Directory.Exists(folderExe)) return;
                // копируем из папки с языком всё в папку EXE с заменой
                _appSetings.CopyFolder(folderNewLang, folderExe);
                #endregion
                #region Audio Game
                // проверяем наличие папки TRANSLATES/AUDIO/<LANG>
                var folderAudioLang = Path.Combine(_gameButtonModel.LanguagesState.PathFolderConfig, "AUDIO", selectedLang.ToString().ToUpper());
                if (Directory.Exists(folderAudioLang))
                {
                    // путь к папке DATA/AUDIO
                    var folderDataAudio = Path.Combine(_gameButtonModel.GameFolderFullPath, "DATA", "AUDIO");
                    if (Directory.Exists(folderDataAudio))
                    {
                        try
                        {
                            // копируем все файлы из AUDIO/<LANG> → DATA/AUDIO
                            _appSetings.CopyFolder(folderAudioLang, folderDataAudio);
                        }
                        catch (Exception exAudio)
                        {
                            var msgAudioError = (Application.Current.TryFindResource("GameViewChangeLangAudioErrorText") as string ??
                                                 "Ошибка при копировании аудио-файлов для языка")
                                                + $" {LangExtensions.GetEnumDescription(selectedLang)} - {exAudio.Message}";
                            var growlInfoAudio = new GrowlInfo
                            {
                                Type = InfoType.Error,
                                Message = msgAudioError,
                                StaysOpen = false,
                                WaitTime = 3
                            };
                            Growl.Error(growlInfoAudio);
                        }
                    }
                }
                #endregion

                // пишем в конфигурационный файл о выбранном новом языке
                var langNewSettings = new GameLanguageConfig()
                {
                    CurrentTranslate = selectedLang.ToString(),
                };
                var json = JsonSerializer.Serialize(langNewSettings);
                File.WriteAllText(_gameButtonModel.LanguagesState.PathToFileConfig, json);
                _gameButtonModel.LanguagesState.Language = selectedLang;
                SelectedLanguage = selectedLang;

                var msg = (Application.Current.TryFindResource("GameViewChangeLangText_1") as string ?? "Смена языка у ")
                    + " " + TitleView + " " +
                    (Application.Current.TryFindResource("GameViewChangeLangPrefixText") as string ?? "на")
                    + " " + LangExtensions.GetEnumDescription(selectedLang) + " "
                    + (Application.Current.TryFindResource("GameViewChangeLangText_2") as string ?? " произошла успешно!");
                var growlInfo = new GrowlInfo
                {
                    Type = InfoType.Success, 
                    Message = msg,
                    StaysOpen = false,
                    WaitTime = 3
                };
                // вызываем Growl с этим объектом
                Growl.Success(growlInfo);
            }
            catch (Exception ex)
            {
                var msgError = (Application.Current.TryFindResource("GameViewChangeLangErrorText") as string ?? "Ошибка смены языка у")
                    + $" {TitleView} " +
                    (Application.Current.TryFindResource("GameViewChangeLangPrefixText") as string ?? "на")
                    + $" {LangExtensions.GetEnumDescription(selectedLang)} - {ex.Message}";

                var growlInfo = new GrowlInfo
                {
                    Type = InfoType.Error,
                    Message = msgError,
                    StaysOpen = false,
                    WaitTime = 3
                };
                // вызываем Growl с этим объектом
                Growl.Error(growlInfo);
            }
        }

        #region Events

        private void StartInstallGame(GameButtonModel model)
        {
            // Если ячейка к которой это событие то применяем изменения к ней
            if (_gameButtonModel.NameMode != model.NameMode)
                return;

            // фиксируем новую будущую версию игры
            _gameButtonModel.NewVersionToServer = model.NewVersionToServer;
            // принудительно обновляем все состояния UI
            NewVersionToServer = _gameButtonModel.NewVersionToServer;

            IsFirstDownloadInstallerStarted = true;
            StartUpdateGame(model.NameMode);
        }

        private void StartUpdateGame(string nameMode)
        {
            // Если ячейка к которой это событие то применяем изменения к ней
            if (_gameButtonModel.NameMode != nameMode)
                return;

            VisibleDownload = Visibility.Visible;
            VisibleBtnDownload = Visibility.Collapsed;
            SetAvailableUpdateStates("download");
            ChangeStateUnpack("");
            ProgressUpdateValue = 0;

            // принудительно обновляем все состояния UI
            StateMode = _gameButtonModel.StateMode;
            // останавливаем фоновые обновления состояния UI
            IsBackgroundUpdateUI = false;
        }

        private void StopUpdateGame(ProcessUpdateGameModel process)
        {
            // Если ячейка к которой это событие то применяем изменения к ней
            if (_gameButtonModel.NameMode != process.NameMode)
                return;

            // включаем фоновые обновления состояния UI
            IsBackgroundUpdateUI = true;

            process.TitleView = TitleView;

            // обновляем версию в UI
            if (process.State == AppUpdater.Chef.StateErrorUpdate.Success)
            {
                if (IsFirstDownloadInstallerStarted)
                    _trayAlertsService.ShowTrayCompleteDownloadProject(process);
                else
                {
                    // запускаем список обновлений если он был после загрузки и установки
                    _eventAggregator.GetEvent<OpenProjectUpdatesInfoEvent>().Publish(_gameButtonModel);

                    _gameButtonModel.LastVersionInstall = process.NewVersion;
                    LastVersion = _gameButtonModel.LastVersionInstall;
                    _trayAlertsService.ShowTrayCompleteUpdateProject(process);
                }
            }

            if (process.State == AppUpdater.Chef.StateErrorUpdate.CancelUpdate)
            {
                if (IsFirstDownloadInstallerStarted)
                    _trayAlertsService.ShowTrayCancelDownloadProject(process);
                else _trayAlertsService.ShowTrayCancelUpdateProject(process);
            }

            if (process.State == AppUpdater.Chef.StateErrorUpdate.NotAvailableUpdate)
            {
                _trayAlertsService.ShowTrayNotAvailableUpdateProject(process);
            }

            // если была установка с нуля
            IsFirstDownloadInstallerStarted = false;

            VisibleDownload = Visibility.Collapsed;
            VisibleBtnDownload = Visibility.Visible;
            SetAvailableUpdateStates("norm");
            ProgressUpdateValue = 0;
            ChangeStateUnpack("");

            // принудительно обновляем все состояния UI
            StateMode = _gameButtonModel.StateMode;

            // перезапускаем сервис мониторинга манифестов
            _eventAggregator.GetEvent<StartManifestMonitorEvent>().Publish();
        }

        public void ProgressUpdateGame(ProcessUpdateGameModel process)
        {
            if (_gameButtonModel.NameMode != process.NameMode)
                return;

            double percentage = process.Progress.DownloadSize / process.Progress.TotalFileSize * 100;
            ProgressUpdateValue = int.Parse(Math.Truncate(percentage).ToString());
        }

        private void SetAdditionalInfoFilesUpdate(ProcessFilesUpdateGameMode process)
        {
            if (_gameButtonModel.NameMode != process.NameMode)
                return;

            FileDownloadQueue = process.UpdateCountFilesHandler.CurrentCountFiles;
            FileDownloadAll = process.UpdateCountFilesHandler.MaxCountFiles;
        }

        private void UnpackFilesGame(ProcessUnpackFilesGamesModel process)
        {
            if (_gameButtonModel.NameMode != process.NameMode)
                return;
            var handle = process.UpdateCountFilesHandler;

            if (handle.TotalDeleteFiles > 0 && handle.CurrentDeleteFilesCount > 0 && handle.TotalDeleteFiles != handle.CurrentDeleteFilesCount)
            {
                ChangeStateUnpack("filesDelete");
                FileDeleteQueue = handle.CurrentDeleteFilesCount;
                FileDownloadAll = handle.TotalDeleteFiles;
            }
            if (handle.TotalDeleteDirs > 0 && handle.CurrentDeleteDirsCount > 0 && handle.TotalDeleteDirs != handle.CurrentDeleteDirsCount)
            {
                ChangeStateUnpack("dirsDelete");
                DirsDeleteQueue = handle.CurrentDeleteDirsCount;
                FileDownloadAll = handle.TotalDeleteDirs;
            }
            if (handle.TotalUnpackFiles == -1 && handle.CurrentUnpackFilesCount > 0)
            {
                ChangeStateUnpack("unpack");
                FileUnpackQueue = handle.CurrentUnpackFilesCount;
            }
            if (handle.TotalUnpackFiles > 0 && handle.CurrentUnpackFilesCount > 0)
            {
                ChangeStateUnpack("unpack_extra");
                FileUnpackQueue = handle.TotalUnpackFiles;
                FileDownloadAll = handle.CurrentUnpackFilesCount;
            }
        }

        private void ChangeStateUnpack(string unpackState)
        {

            switch(unpackState)
            {
                case "unpack":
                    IsUnpackFiles = Visibility.Visible;
                    IsDeleteFiles = Visibility.Collapsed;
                    IsDeleteDirs = Visibility.Collapsed;
                    IsDownloadFirstProcess = Visibility.Collapsed;
                    break;
                case "unpack_extra":
                    IsUnpackFiles = Visibility.Visible;
                    IsDeleteFiles = Visibility.Collapsed;
                    IsDeleteDirs = Visibility.Collapsed;
                    IsDownloadFirstProcess = Visibility.Visible;
                    break;
                case "filesDelete":
                    IsUnpackFiles = Visibility.Collapsed;
                    IsDeleteFiles = Visibility.Visible;
                    IsDeleteDirs = Visibility.Collapsed;
                    IsDownloadFirstProcess = Visibility.Collapsed;
                    break;
                case "dirsDelete":
                    IsUnpackFiles = Visibility.Collapsed;
                    IsDeleteFiles = Visibility.Collapsed;
                    IsDeleteDirs = Visibility.Visible;
                    IsDownloadFirstProcess = Visibility.Collapsed;
                    break;
                case "":
                default:
                    IsDownloadFirstProcess = Visibility.Visible;
                    IsUnpackFiles = Visibility.Collapsed;
                    IsDeleteFiles = Visibility.Collapsed;
                    IsDeleteDirs = Visibility.Collapsed;
                    break;
            }
        }

        private void SetAvailableUpdateStates(string state)
        {
            currentDownloadState = state;
            switch (state)
            {
                case "norm":
                    NeedUpdateStateText = (Application.Current.TryFindResource("NeedUpdateStateText") as string
                            ?? $"Доступно обновление!");
                    break;
                case "download":
                    NeedUpdateStateText = (Application.Current.TryFindResource("DownloadUpdateStateText") as string
                           ?? $"Скачивается...");
                    break;
            }
        }

        #endregion

        #region WinAPI
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_RESTORE = 9;
        #endregion
    }
}
