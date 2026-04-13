/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 13 апреля 2026 13:12:10
 * Version: 1.0.18
 */

using System.IO;
using System.Windows;

using LizeriumSteam.Events;
using LizeriumSteam.Services.Games.GameInstallTrackerService;
using LizeriumSteam.Services.Games.GameInstallTrackerService.Implennts;
using LizeriumSteam.Services.Games.GameManifestMonitorService;
using LizeriumSteam.Services.Games.GameManifestMonitorService.Implennts;
using LizeriumSteam.Services.Games.GameProjectsService;
using LizeriumSteam.Services.Games.GameProjectsService.Implennts;
using LizeriumSteam.Services.Games.GameSaveManagerService;
using LizeriumSteam.Services.Games.GameSaveManagerService.Implennts;
using LizeriumSteam.Services.Games.GameUpdateService;
using LizeriumSteam.Services.Games.GameUpdateService.Implements;
using LizeriumSteam.Services.Games.ProjectMonitorService;
using LizeriumSteam.Services.Games.ProjectMonitorService.Implennts;
using LizeriumSteam.Services.Lang;
using LizeriumSteam.Services.Lang.Implements;
using LizeriumSteam.Services.Settings;
using LizeriumSteam.Services.Settings.Implements;
using LizeriumSteam.Services.Tray;
using LizeriumSteam.Services.Tray.Implements;
using LizeriumSteam.Services.Tray.Windows;
using LizeriumSteam.Services.Update;
using LizeriumSteam.Services.Update.implements;
using LizeriumSteam.ViewModels;
using LizeriumSteam.Views;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Prism.DryIoc;
using Prism.Events;
using Prism.Ioc;
using Prism.Services.Dialogs;

using Serilog;

namespace LizeriumSteam
{
    public class Bootstrapper : PrismBootstrapper
    {
        private IProjectActivityMonitor _projectActivityMonitor;
        private IGameInstallTrackerService _gameInstallTrackerService;
        private IAppUpdateService _appUpdateService;
        private ITrayService _trayService;
        private IGameManifestMonitorService _gameManifestMonitorService;

        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // логирование
            var services = new ServiceCollection();
            var pathLogSettings = Path.Combine(Path.GetDirectoryName(typeof(MainView).Assembly.Location), $"Logs/app-.log");
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Debug);

                Log.Logger = new LoggerConfiguration()
                   .MinimumLevel.Debug()
                   .WriteTo.File(
                        pathLogSettings, 
                       rollingInterval: RollingInterval.Day,
                       retainedFileCountLimit: 10,
                       rollOnFileSizeLimit: true,
                       shared: true
                   )
                   .CreateLogger();

                builder.AddSerilog(Log.Logger, dispose: true);
            });
            var provider = services.BuildServiceProvider();
            // Регистрируем ILoggerFactory
            containerRegistry.RegisterInstance(provider.GetRequiredService<ILoggerFactory>());
            // Универсальная регистрация для ILogger<T>
            containerRegistry.RegisterSingleton(typeof(ILogger<>), typeof(Logger<>));

            var pathGlobalSettings = Path.Combine(Path.GetDirectoryName(typeof(MainView).Assembly.Location), "Settings/GlobalSettings.json");
            // глобальные настройки
            containerRegistry.RegisterSingleton<IAppSettings>(() => {
                var logger = provider.GetRequiredService<ILogger<GlobalSettings>>();
                return new GlobalSettings(pathGlobalSettings, logger);
            });
            // выбор языковых параметров
            containerRegistry.RegisterSingleton<ILanguageService, LanguageService>();
            // отображение в windows трее
            containerRegistry.RegisterSingleton<ITrayService, TrayService>();
            // отображение уведомлений системы в Windows
            containerRegistry.RegisterSingleton<ITrayAlertsService, TrayAlertsService>();
            // сервис загрузки и обновления информации о проектах
            containerRegistry.RegisterSingleton<IProjectsService, GameProjectsService>();
            // сервис мониторинга актального состояния настроек проектов
            containerRegistry.RegisterSingleton<IGameManifestMonitorService, GameManifestMonitorService>();
            // сервис загрузки и создания сохранений проектов
            containerRegistry.RegisterSingleton<IGameSaveManagerService, GameSaveManagerService>();
            // сервис загрузки и обновления проектов
            containerRegistry.RegisterSingleton<IProjectUpdateService, ProjectUpdateService>();
            // сервис мониторинга и запуска готовных у становке проектов
            containerRegistry.RegisterSingleton<IGameInstallTrackerService, GameInstallTrackerService>();
            // регистрация сервиса автообновления
            containerRegistry.RegisterSingleton<IAppUpdateService, AppUpdateService>();
            // регистрация сервиса мониторинга активных проектов 
            containerRegistry.RegisterSingleton<IProjectActivityMonitor, ProjectActivityMonitorService>();

            // окно закрытия приложения
            containerRegistry.RegisterDialog<CloseView, CloseDialogWindowModel>();
            // окно выбора языковых параметров
            containerRegistry.RegisterDialog<LanguageSelectionDialogView, LanguageSelectionDialogViewModel>();
            // окно игр
            containerRegistry.RegisterSingleton<GameButtonViewModelFactory>();
            containerRegistry.Register<GamesView>();
            containerRegistry.Register<GameViewModel>();
            containerRegistry.Register<GameButtonView>();
            containerRegistry.Register<GameButtonViewModel>();
        }

        protected override async void OnInitialized()
        {
            base.OnInitialized();
            var appSettings = Container.Resolve<IAppSettings>();
            var eventAggregator = Container.Resolve<IEventAggregator>();
            // инициализируем настройки приложения
            appSettings.Load();

            //инициализируем выбор языка приложения
            var languageService = Container.Resolve<ILanguageService>();
            string lang = appSettings.AppGlobalSettings.Language; // читаем сохранённый язык

            if (string.IsNullOrEmpty(lang))
            {
                // если ещё не выбран, показываем диалог
                var dialogService = Container.Resolve<IDialogService>();
                var parameters = new DialogParameters { { "DefaultLanguage", "ru" } };

                dialogService.ShowDialog("LanguageSelectionDialogView", parameters, result =>
                {
                    if (result.Result == ButtonResult.OK)
                    {
                        lang = result.Parameters.GetValue<string>("SelectedLanguage");
                    }
                    else
                    {
                        lang = parameters.GetValue<string>("DefaultLanguage");
                    }
                    eventAggregator.GetEvent<LanguageChangedEvent>().Publish(lang);
                });
            }
            else eventAggregator.GetEvent<LanguageChangedEvent>().Publish(lang);

            // проверка и установка новых ключей конфигурации
            Container.Resolve<IAppUpdateService>().CheckConfigurationAndCompare();
            // инициализация трея
            _trayService = Container.Resolve<ITrayService>();
            _trayService.Initialize();
            // инициализируем сервис автообновления приложения
            _appUpdateService = Container.Resolve<IAppUpdateService>();
            _appUpdateService.InitializeAutoUpdate();
            // инициализируем сервис мониторинга и запуска готовных у становке проектов
            _gameInstallTrackerService = Container.Resolve<IGameInstallTrackerService>();
            // инициализируем сервис мониторинга активных проектов
            _projectActivityMonitor = Container.Resolve<IProjectActivityMonitor>();
            // инициализируем сервис мониторинга актального состояния настроек проектов
            _gameManifestMonitorService = Container.Resolve<IGameManifestMonitorService>();
            _gameManifestMonitorService.StartMonitoring();
        }

        public void StopServices()
        {
            _trayService?.Dispose();
            _appUpdateService?.StopAutoUpdate();
            _projectActivityMonitor?.StopMonitoring();
            _gameManifestMonitorService?.StopMonitoring();
            _gameInstallTrackerService?.Stop();
        }
    }
}
