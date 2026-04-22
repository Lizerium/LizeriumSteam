/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 22 апреля 2026 18:58:37
 * Version: 1.0.29
 */

using System;
using System.IO;
using System.Reflection;

using AppUpdater;
using AppUpdater.Chef;

using LizeriumSteam.Events;

using Microsoft.Extensions.Logging;

using Prism.Events;

using static System.Windows.Forms.AxHost;

namespace LizeriumSteam.Services.Update.implements
{
    /// <summary>
    /// Система автоматического обновления приложения
    /// </summary>
    public class AppUpdateService : IAppUpdateService
    {
        private readonly ILogger<AppUpdateService> _logger;
        private readonly IEventAggregator _eventAggregator;
        private AutoUpdater _autoUpdater;

        public AppUpdateService(ILogger<AppUpdateService> logger,
            IEventAggregator eventAggregator)
        {
            _logger = logger;
            _eventAggregator = eventAggregator;
        }

        public void InitializeAutoUpdate()
        {
            try
            {
                string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string baseDir = Path.Combine(appPath, "..\\");
                _logger.LogInformation($"Инициализация системы мониторинга обновлений: {baseDir}");

                _autoUpdater = new AutoUpdater(UpdateManager.Default);
                _autoUpdater.SecondsBetweenChecks = 10;
                _autoUpdater.Updated += new EventHandler<StateErrorUpdate>(AutoUpdater_Updated);
                _autoUpdater.EnableUpdated += new EventHandler<string>(NewVersion_EnableUpdated);
                _autoUpdater.PercentUpdate += AutoUpdater_PercentUpdate;
                _autoUpdater.Start();
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] InitializeAutoUpdate::" + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// Проверка и установка новых ключей конфигурации
        /// (если те были в последнем config.xml)
        /// </summary>
        public void CheckConfigurationAndCompare()
        {
            try
            {
                var result = UpdateManager.Default.CheckAndCompareConfigs();
                _logger.LogInformation("\n Информация о проверке разницы между старой и актуальной конфигурацией: \n" + result.TextMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] Check Configuration And Compare - " + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
        }

        private void AutoUpdater_PercentUpdate(object sender, double percent)
        {
            _eventAggregator.GetEvent<AppUpdateProgressEvent>().Publish((int)percent);
        }

        private void NewVersion_EnableUpdated(object sender, string version)
        {
            _eventAggregator.GetEvent<AppUpdateAvailableEvent>().Publish(version);
        }

        private void AutoUpdater_Updated(object sender, StateErrorUpdate state)
        {
            _eventAggregator.GetEvent<AppUpdateCompletedEvent>().Publish(state);
        }

        public void StopAutoUpdate()
        {
            _autoUpdater.Stop();
        }
    }
}
