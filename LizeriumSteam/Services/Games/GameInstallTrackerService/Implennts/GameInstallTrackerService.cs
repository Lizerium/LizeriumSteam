/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 01 мая 2026 07:14:08
 * Version: 1.0.38
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using HandyControl.Controls;
using HandyControl.Data;

using LizeriumSteam.Events;
using LizeriumSteam.Models.Games;
using LizeriumSteam.Models.Games.GameInstall;
using LizeriumSteam.Services.Lang.Implements;
using LizeriumSteam.Services.Tray.Windows;

using Microsoft.Extensions.Logging;

using Prism.Events;

namespace LizeriumSteam.Services.Games.GameInstallTrackerService.Implennts
{
    /// <summary>
    /// Сервис мониторинга и запуска готовных у становке проектов
    /// </summary>
    public class GameInstallTrackerService : IGameInstallTrackerService, IDisposable
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ITrayAlertsService _trayAlertsService;
        private readonly ILogger<GameInstallTrackerService> _logger;
        private CancellationTokenSource _cts;

        public GameInstallTrackerService(IEventAggregator eventAggregator,
            ILogger<GameInstallTrackerService> logger,
            ITrayAlertsService trayAlertsService) 
        {
            _eventAggregator = eventAggregator;
            _trayAlertsService = trayAlertsService;
            _logger = logger;

            #region events

            // обнаружение обновления
            _eventAggregator.GetEvent<AppGameIsAvailableToInstallEvent>()
                .Subscribe(async v => await AvailableToInstallAsync(v), ThreadOption.UIThread);

            #endregion
        }

        private async Task AvailableToInstallAsync(ProcessInstallGameModel processInstall)
        {
            try
            {
                // Оповещение UI о начале установки проекта
                var msg = (Application.Current.TryFindResource("AppDeployAvailableUIAlertText") as string ?? "Файлы скачаны, начните установку -")
                   + " " + processInstall.GameButtonModel.TitleView;
                var growlInfo = new GrowlInfo
                {
                    Type = InfoType.Success,
                    Message = msg,
                    StaysOpen = false,
                    WaitTime = 7
                };
                // вызываем Growl с этим объектом
                Growl.Success(growlInfo);

                // открываю папку со скачанным проектом
                Process.Start("explorer.exe", processInstall.CurrentStartupProjectFolder);
                // проверяю существование файла установки 
                if (!File.Exists(processInstall.CurrentStartupFileInstaller))
                {
                    _trayAlertsService.ShowTrayNotCompleteInstallProject(processInstall.GameButtonModel);
                    return;
                }

                // Запуск процесса установки скачанного проекта
                Process.Start(processInstall.CurrentStartupFileInstaller);
                var process = Process.GetProcessesByName(processInstall.GameButtonModel.NameFileInstallerNotExt);
                // Ожидание завершения процесса установки проекта пользователем чтобы показать обновления
                while (process != null && process.Length > 0)
                {
                    process = Process.GetProcessesByName(processInstall.GameButtonModel.NameFileInstallerNotExt);
                    await Task.Delay(2000);
                    await Task.Yield();
                }

                // запускаем список обновлений если он был после загрузки и установки
                _eventAggregator.GetEvent<OpenProjectUpdatesInfoEvent>().Publish(processInstall.GameButtonModel);
                // оповещаем уведомлением о завершении установки
                _trayAlertsService.ShowTrayCompleteInstallProject(processInstall.GameButtonModel);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Ошибка установки проекта.");
            }
        }

        public void Stop()
        {
            _cts?.Cancel();
        }

        public void Dispose()
        {
            Stop();
            _cts?.Dispose();
        }
    }
}
