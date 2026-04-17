/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 17 апреля 2026 07:02:44
 * Version: 1.0.24
 */

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

using AppUpdater.Chef;

using LizeriumSteam.Events;
using LizeriumSteam.Models.Games.GameUpdate;
using LizeriumSteam.Services.Lang;
using LizeriumSteam.Services.Settings;
using LizeriumSteam.Services.Tray.Windows;

using Microsoft.Extensions.Logging;

using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace LizeriumSteam.ViewModels
{
    public class MainViewModel : BindableBase
    {

        #region Props

        private string _title = "Lizerium Steam";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _icon = "/Resources/logo.ico";

        public string Icon
        {
            get { return _icon; }
            set { _icon = value; }
        }

        private int _progress = 100;

        public int LoadingProgress
        {
            get { return _progress; }
            set { SetProperty(ref _progress, value); }
        }

        private Visibility _progressVisible = Visibility.Collapsed;

        public Visibility ProgressVisible
        {
            get { return _progressVisible; }
            set { SetProperty(ref _progressVisible, value); }
        }

        private string _statusTextLoading = (Application.Current.TryFindResource("MainViewWaitStatusText") as string
                            ?? $"");

        public string StatusTextLoading
        {
            get { return _statusTextLoading; }
            set { SetProperty(ref _statusTextLoading, value); }
        }

        private string _statusSpeedLoading = "0 МБ/c";

        public string StatusSpeedLoading
        {
            get { return _statusSpeedLoading; }
            set { SetProperty(ref _statusSpeedLoading, value); }
        }

        private Visibility _statusSpeedLoadingVisible = Visibility.Collapsed;

        public Visibility StatusSpeedLoadingVisible
        {
            get { return _statusSpeedLoadingVisible; }
            set { SetProperty(ref _statusSpeedLoadingVisible, value); }
        }

        private bool isNeedRestarted = false;

        public bool IsNeedRestarted
        {
            get => isNeedRestarted;
            set => SetProperty(ref isNeedRestarted, value);
        }

        private bool isStarted = false;

        private string _version;

        public string Version
        {
            get => _version;
            set => SetProperty(ref _version, value);
        }

        private Visibility _isReadyApp = Visibility.Hidden;

        public Visibility IsReadyApp
        {
            get => _isReadyApp; 
            set => SetProperty(ref _isReadyApp, value);
        }

        private string _currentLanguage;

        public string CurrentLanguage
        {
            get { return _currentLanguage; }
            set
            {
                if (SetProperty(ref _currentLanguage, value))
                {
                    RaisePropertyChanged(nameof(IsRuEnabled));
                    RaisePropertyChanged(nameof(IsEnEnabled));
                }
            }
        }

        public bool IsRuEnabled => !string.Equals(CurrentLanguage, "ru", StringComparison.OrdinalIgnoreCase);
        public bool IsEnEnabled => !string.Equals(CurrentLanguage, "en", StringComparison.OrdinalIgnoreCase);

        #endregion

        public ObservableCollection<ContactItemViewModel> Contacts { get; }
        = new ObservableCollection<ContactItemViewModel>();
        public readonly IDialogService DialogService;
        public StateErrorUpdate LastStateUpdate { get; private set; } = StateErrorUpdate.Wait;

        private readonly IAppSettings _appSettings;
        private readonly ITrayAlertsService _trayAlertsService;
        private readonly ILogger<MainViewModel> _logger;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILanguageService _languageService;

        public DelegateCommand OpenWishCommand { get; }
        public DelegateCommand OpenMainSiteCommand { get; }
        public DelegateCommand OpenDiscordCommand { get; }
        public DelegateCommand OpenVKCommand { get; }

        public MainViewModel(IDialogService dialogService,
            IAppSettings appSettings,
            ILogger<MainViewModel> logger,
            ILanguageService languageService,
            ITrayAlertsService trayAlertsService,
            IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            DialogService = dialogService;
            _appSettings = appSettings;
            _trayAlertsService = trayAlertsService;
            _languageService = languageService;
            _logger = logger;

            _appSettings.SettingsLoaded += AppSettings_SettingsLoaded;

            #region events

            // обнаружение обновления
            _eventAggregator.GetEvent<AppUpdateAvailableEvent>()
                .Subscribe(v => UpdateAvailable(v));

            // прогресс обновления
            _eventAggregator.GetEvent<AppUpdateProgressEvent>()
                .Subscribe(p => UpdateProgress(p));

            // завершение обновления
            _eventAggregator.GetEvent<AppUpdateCompletedEvent>()
                .Subscribe(state => CompleteLoadingUpdate(state));

            // Подписка на событие изменения языка
            _eventAggregator.GetEvent<LanguageChangedEvent>().Subscribe(OnLanguageChanged);

            // подписываемся на событие готовности
            _eventAggregator.GetEvent<AppReadyEvent>()
                .Subscribe(OnAppReady, ThreadOption.UIThread);

            // мониторинг скорости скачивания файлов
            _eventAggregator.GetEvent<AppGameUpdateProcessEvent>()
             .Subscribe(ViewSpeedDownload, ThreadOption.UIThread);

            _eventAggregator.GetEvent<AppGameUpdateStopEvent>()
                .Subscribe(EndViewSpeedDownload, ThreadOption.UIThread);

            #endregion

            OpenWishCommand = new DelegateCommand(OpenWish);
            OpenMainSiteCommand = new DelegateCommand(OpenMainSite);
            OpenDiscordCommand = new DelegateCommand(OpenDiscord);
            OpenVKCommand = new DelegateCommand(OpenVK);
        }

        private void OnAppReady()
        {
            IsReadyApp = Visibility.Visible;
        }

        private void OpenDiscord()
        {
            var psi = new ProcessStartInfo
            {
                FileName = "https://discord.gg/GPTyXja4",
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void OpenVK()
        {
            var psi = new ProcessStartInfo
            {
                FileName = "https://vk.com/lizerium_game",
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void OpenMainSite()
        {
            var psi = new ProcessStartInfo
            {
                FileName = "https://lizup.ru/Home/Game",
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void OpenWish()
        {
            var lizMode = _appSettings.GameButtonInfo.FirstOrDefault(it => it.NameMode
                == "LizeriumFreelancerMode");
            if (lizMode != null)
            {
                if(!string.IsNullOrEmpty(lizMode.SupportUrl))
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = lizMode.SupportUrl,
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                }
            }
        }

        private void OnLanguageChanged(string newLang)
        {
            if (!string.IsNullOrEmpty(newLang))
            {
                _languageService.ApplyLanguage(newLang);
                _appSettings.AppGlobalSettings.Language = newLang;
                CurrentLanguage = newLang;
                _appSettings.Save();

                LoadContacts();
                CompleteLoadingUpdate(LastStateUpdate);

                _logger.LogInformation($"Language changed to {newLang}");
                _eventAggregator.GetEvent<LangChangedCompleteEvent>().Publish();
            }
        }

        private void AppSettings_SettingsLoaded()
        {
            LoadContacts();
        }

        private void LoadContacts()
        {
            var lang = _appSettings.AppGlobalSettings.Language;

            Contacts.Clear();
            if (_appSettings?.AppGlobalSettings?.Contacts != null)
            {
                foreach (var contact in _appSettings.AppGlobalSettings.Contacts)
                {
                    Contacts.Add(new ContactItemViewModel(contact, lang));
                }
            }

            _logger.LogInformation("Контакты загружены");
        }

        private void UpdateAvailable(string version)
        {
            try
            {
                IsReadyApp = Visibility.Visible;
                StatusTextLoading = (Application.Current.TryFindResource("MainViewUpdateAvailableText_1") as string
                            ?? $"Скачивается версия  💢 ") + $" {version} "
                            + (Application.Current.TryFindResource("MainViewUpdateAvailableText_2") as string
                            ?? $" 💢 Пожалуйста подождите...");
                LoadingProgress = 0;
                ProgressVisible = Visibility.Visible;
                // оповещение в системном трее
                _trayAlertsService.ShowTrayUpdateAvailable(version);
                // логирование
                _logger.LogInformation("Обновление доступно: " + version);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] NewVersion_EnableUpdated::" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
        }

        private void UpdateProgress(int progress)
        {
            LoadingProgress = progress;
        }

        private void CompleteLoadingUpdate(StateErrorUpdate state)
        {
            try
            {
                ProgressVisible = Visibility.Collapsed;
                LastStateUpdate = state;
                Version = _appSettings.AppGlobalSettings.Version;

                switch (state)
                {
                    case StateErrorUpdate.Success:
                        IsNeedRestarted = true;
                        StatusTextLoading = Application.Current.TryFindResource("MainViewStateErrorUpdateSuccessText") as string 
                            ?? $"Обновлено до последней версии. 💫 Пожалуйста перезапустите приложение!";
                        if (!isStarted)
                        {
                            _eventAggregator.GetEvent<AppReadyEvent>().Publish();
                            isStarted = true;
                        }
                        break;
                    case StateErrorUpdate.ErrorConnectServer:
                        StatusTextLoading = Application.Current.TryFindResource("MainViewStateErrorUpdateErrorConnectServerText") as string
                            ?? $"⛔ Не обновлено! Ошибка подключения! ⛔";
                        break;
                    case StateErrorUpdate.ErrorSaveFile:
                        StatusTextLoading = Application.Current.TryFindResource("MainViewStateErrorUpdateErrorSaveFileText") as string
                            ?? $"⛔ Не обновлено! Ошибка сохранения файла! ⛔";
                        break;
                    case StateErrorUpdate.NotAvailableUpdate:
                        StatusTextLoading = (Application.Current.TryFindResource("MainViewStateErrorUpdateNotAvailableUpdateText_1") as string
                            ?? $"💫 Версия: ") + $" {_appSettings.AppGlobalSettings.Version} " 
                            + ((isNeedRestarted ? Application.Current.TryFindResource("MainViewStateErrorUpdateNotAvailableUpdateText_2") as string
                            ?? $" 💫 Пожалуйста перезапустите приложение!" : ""));
                        if (!isStarted)
                        {
                            _eventAggregator.GetEvent<AppReadyEvent>().Publish();
                            isStarted = true;
                        }
                        break;
                }

                // оповещение в системном трее
                _trayAlertsService.ShowTrayCompleteUpdate(state);
                // логирование
                _logger.LogInformation($"Обновление: {StatusTextLoading} | Состояние: {state}");
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] AutoUpdater_Updated::" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
        }

        private void ViewSpeedDownload(ProcessUpdateGameModel process)
        {
            if (process.Progress.DownloadSize < process.Progress.TotalFileSize - 1)
            {
                StatusSpeedLoadingVisible = Visibility.Visible;
                StatusSpeedLoading = $"{process.Progress.DownloadSpeed:F2} МБ/c";
            }
            else
            {
                StatusSpeedLoadingVisible = Visibility.Collapsed;
                StatusSpeedLoading = "";
            }
        }

        private void EndViewSpeedDownload(ProcessUpdateGameModel model)
        {
            StatusSpeedLoadingVisible = Visibility.Collapsed;
            StatusSpeedLoading = "";
        }
    }
}
