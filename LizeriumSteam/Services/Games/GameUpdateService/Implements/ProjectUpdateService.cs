/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 10 мая 2026 08:03:59
 * Version: 1.0.47
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Xml;

using AppUpdater;
using AppUpdater.Chef;
using AppUpdater.Server;

using HandyControl.Controls;
using HandyControl.Data;

using LizeriumSteam.Events;
using LizeriumSteam.Events.Unpack;
using LizeriumSteam.Models.Games;
using LizeriumSteam.Models.Games.GameInstall;
using LizeriumSteam.Models.Games.GameUpdate;
using LizeriumSteam.Services.Lang.Implements;

using Microsoft.Extensions.Logging;

using Prism.Events;

namespace LizeriumSteam.Services.Games.GameUpdateService.Implements
{
    /// <summary>
    /// Cервис загрузки и обновления проектов
    /// </summary>
    public class ProjectUpdateService : IProjectUpdateService
    {
        private IEventAggregator _eventAggregator;
        private ILogger<ProjectUpdateService> _logger;
        private CancellationTokenSource _cts;
        private int _isProcessUpdate = 0; 
        private string _projectName;
        private string _newVersion;
        private string _oldVersion;

        #region Install settings
        private string _currentStartupFileInstaller;
        private string _currentStartupProjectFolder;
        private GameButtonModel _currentInstallProject;
        #endregion

        public ProjectUpdateService(IEventAggregator eventAggregator,
            ILogger<ProjectUpdateService> logger)
        {
            _eventAggregator = eventAggregator;
            _logger = logger;
        }

        /// <summary>
        /// Установка первичной версии приложения
        /// 
        /// Папка установки
        /// C:\Users\<User>\AppData\Local\<Project>
        /// </summary>
        public void InstallProject(GameButtonModel gameButtonModel)
        {
            // если уже идёт обновление — выходим
            if (Interlocked.CompareExchange(ref _isProcessUpdate, 1, 0) == 1)
                return;

            try
            {
                if (_cts != null)
                {
                    _cts.Cancel();
                    _cts = null;
                }
                _cts = new CancellationTokenSource();
                
                _currentStartupFileInstaller = Path.Combine(Path.GetTempPath(), gameButtonModel.NameMode, gameButtonModel.NameFileInstaller);
                _currentStartupProjectFolder = Path.Combine(Path.GetTempPath(), gameButtonModel.NameMode);
                var clearAllFilesPath = Path.Combine(Path.GetTempPath(), gameButtonModel.NameMode);

                //очищаю старые данные игры из Temp папки
                UpdateManager.Default.ClearUpdateDir(clearAllFilesPath);

                UpdateManager.Default.UpdateServer.VersionReceive = null;
                UpdateManager.Default.UpdateServer.VersionReceive += ReceiveVersionWait;
                //получаю последнюю версию установщика мода
                var lastVersion = UpdateManager.Default.UpdateServer.GetLastVersionMode(gameButtonModel.NameMode);

                //сохраняю модель для системы оповещения трекера установки 
                _currentInstallProject = gameButtonModel;
              
                //помечаю информацию для системы оповещений UI
                _projectName = gameButtonModel.NameMode;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Установка проекта оборвалась");
            }
        }

        private async void ReceiveVersionWait(object sender, string data)
        {
            try
            {
                var version = string.Empty;
                if (!string.IsNullOrEmpty(data))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(data);
                    version = doc.SelectSingleNode("config/version").InnerText;

                    _currentInstallProject.NewVersionToServer = version;
                    _newVersion = version;
                    _oldVersion = version;
                }
                else
                {
                    // оповещаем систему о завершении скачивания
                    EndInstall(StateErrorUpdate.NotAvailableUpdate);
                    return;
                }

                // оповещаем ячейку с проектом и UI о том что начато скачивание её установщика 
                _eventAggregator.GetEvent<AppGameInstallStartEvent>().Publish(_currentInstallProject);

                UpdateManager.Default.DataDownloadHandle = null;
                UpdateManager.Default.DataDownloadHandle += UpdateModProgressDownloadHandle;
                UpdateManager.Default.DataUnpackHandle = null;
                UpdateManager.Default.DataUnpackHandle += UpdateUnpackStatsHandle;
                UpdateManager.Default.EndCookHandle = null;
                UpdateManager.Default.EndCookHandle += EndCookHandle;

                //качаю и расшифровываю 
                var info = new UpdateInfo(true, version);
                UpdateManager.Default.UpdateCountFilesHandler = null;
                UpdateManager.Default.UpdateCountFilesHandler += UpdateModCountFilesHandler;

                bool isUpdate = await UpdateManager.Default.DoInstallMode(_cts.Token,
                    info, "1.0.0", version, _currentInstallProject.NameMode);

                if (!isUpdate)
                {
                    // оповещаем систему о завершении скачивания
                    EndInstall(StateErrorUpdate.NotAvailableUpdate);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Установка проекта, получение статистики оборвалось");
            }
        }

        /// <summary>
        /// Запуск установки приложения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EndCookHandle(object sender, StateErrorUpdate e)
        {
            if (e == StateErrorUpdate.Success)
            {
                // Оповещаем трекер о готовности проекта к установке
                _eventAggregator.GetEvent<AppGameIsAvailableToInstallEvent>().Publish(new ProcessInstallGameModel()
                {
                    CurrentStartupProjectFolder = _currentStartupProjectFolder,
                    GameButtonModel = _currentInstallProject,
                    CurrentStartupFileInstaller = _currentStartupFileInstaller
                });
            }

            // оповещаем систему о завершении скачивания
            EndInstall(e);
        }

        private void EndInstall(StateErrorUpdate e)
        {
            _eventAggregator.GetEvent<AppGameUpdateStopEvent>().Publish(new ProcessUpdateGameModel()
            {
                NameMode = _projectName,
                Progress = null,
                NewVersion = _newVersion,
                State = e,
                OldVersion = _oldVersion
            });

            UpdateManager.Default.DataDownloadHandle -= UpdateModProgressDownloadHandle;
            UpdateManager.Default.DataUnpackHandle -= UpdateUnpackStatsHandle;
            UpdateManager.Default.UpdateAppCountHandler -= UpdateModCountFilesHandler;
            UpdateManager.Default.EndCookHandle -= EndCookHandle;

            // сброс флага
            Interlocked.Exchange(ref _isProcessUpdate, 0);
            _projectName = null;
        }

        /// <summary>
        /// Обновление проекта
        /// </summary>
        /// <param name="game">Данные проекта</param>
        public async Task UpdateProject(GameButtonModel game)
        {
            // если уже идёт обновление — выходим
            if (Interlocked.CompareExchange(ref _isProcessUpdate, 1, 0) == 1)
                return;

            try
            {
                if (_cts != null)
                {
                    _cts.Cancel();
                    _cts = null;
                }
                _cts = new CancellationTokenSource();

                // оповещаем ячейку с проектом и UI о том что начато её обновление 
                _eventAggregator.GetEvent<AppGameUpdateStartEvent>().Publish(game.NameMode);
                _projectName = game.NameMode;
                _oldVersion = game.GameInstallVersion;
                _newVersion = game.NewVersionToServer;

                UpdateManager.Default.DataDownloadHandle = null;
                UpdateManager.Default.DataDownloadHandle += UpdateModProgressDownloadHandle;
                UpdateManager.Default.DataUnpackHandle = null;
                UpdateManager.Default.DataUnpackHandle += UpdateUnpackStatsHandle;
                UpdateManager.Default.UpdateAppCountHandler = null;
                UpdateManager.Default.UpdateAppCountHandler += UpdateModCountFilesHandler;
                UpdateManager.Default.EndUpdateAppHandler = null;
                UpdateManager.Default.EndUpdateAppHandler += EndUpdateAppHandler;

                UpdateManager.Default.SuccessUnpackTarOperationHandle = null;
                UpdateManager.Default.SuccessUnpackTarOperationHandle += SuccessUnpackTarOperationHandler;
                UpdateManager.Default.SuccessUnpack7zOperationHandle = null;
                UpdateManager.Default.SuccessUnpack7zOperationHandle += SuccessUnpack7zOperationHandler;
                UpdateManager.Default.WarningUnpack7zOperationHandle = null;
                UpdateManager.Default.WarningUnpack7zOperationHandle += WarningUnpack7zOperationHandler;
                UpdateManager.Default.WarningLowTimeOperationHandle = null;
                UpdateManager.Default.WarningLowTimeOperationHandle += WarningLowTimeOperationHandler;

                Exception exception = await UpdateManager.Default.GetUpdatesMode(_cts.Token,
                    game.GameInstallVersion,
                    game.NewVersionToServer,
                    game.NameMode,
                    game.GameFolderFullPath);

                if (exception != null)
                {
                    _logger.LogError(exception, "Установка обновления оборвалась");
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Установка обновления оборвалась...");
            }
            finally
            {
                // отписка от событий
                UpdateManager.Default.DataDownloadHandle -= UpdateModProgressDownloadHandle;
                UpdateManager.Default.DataUnpackHandle -= UpdateUnpackStatsHandle;
                UpdateManager.Default.UpdateAppCountHandler -= UpdateModCountFilesHandler;
                UpdateManager.Default.EndUpdateAppHandler -= EndUpdateAppHandler;
                UpdateManager.Default.SuccessUnpackTarOperationHandle -= SuccessUnpackTarOperationHandler;
                UpdateManager.Default.SuccessUnpack7zOperationHandle -= SuccessUnpack7zOperationHandler;
                UpdateManager.Default.WarningUnpack7zOperationHandle -= WarningUnpack7zOperationHandler;
                UpdateManager.Default.WarningLowTimeOperationHandle -= WarningLowTimeOperationHandler;

                // сброс флага
                Interlocked.Exchange(ref _isProcessUpdate, 0);
                _projectName = null;
            }
        }

        private void WarningLowTimeOperationHandler(object sender, EventArgs e)
        {
            _eventAggregator.GetEvent<WarningLowTimeOperationEvent>().Publish();
        }

        private void WarningUnpack7zOperationHandler(object sender, EventArgs e)
        {
            _eventAggregator.GetEvent<WarningUnpackZipOperationEvent>().Publish();
        }

        private void SuccessUnpack7zOperationHandler(object sender, EventArgs e)
        {
            _eventAggregator.GetEvent<SuccessUnpackZipOperationEvent>().Publish();
        }

        private void SuccessUnpackTarOperationHandler(object sender, EventArgs e)
        {
            _eventAggregator.GetEvent<SuccessUnpackTarOperationEvent>().Publish();
        }

        /// <summary>
        /// Остановка обновления проекта
        /// </summary>
        /// <param name="game">Данные проекта</param>
        public void StopUpdateProject(GameButtonModel game)
        {
            _cts.Cancel();
            UpdateManager.Default.StopInstallMode();
        }

        public void StopUpdates()
        {
            _cts?.Cancel();
        }

        private void EndUpdateAppHandler(object sender, StateErrorUpdate e)
        {
            if(!string.IsNullOrEmpty(_projectName))
                _eventAggregator.GetEvent<AppGameUpdateStopEvent>().Publish(new ProcessUpdateGameModel()
                {
                    NameMode = _projectName,
                    Progress = null,
                    NewVersion = _newVersion,
                    State = e,
                    OldVersion = _oldVersion
                });
        }

        private void UpdateModProgressDownloadHandle(object sender, DataDownloadHandle e)
        {
            if (!string.IsNullOrEmpty(_projectName))
                _eventAggregator.GetEvent<AppGameUpdateProcessEvent>().Publish(new ProcessUpdateGameModel()
                {
                    NameMode = _projectName,
                    Progress = e,
                    NewVersion = _newVersion,
                    OldVersion = _oldVersion
                });
        }

        private void UpdateModCountFilesHandler(object sender, UpdateCountFilesHandler e)
        {
            if (!string.IsNullOrEmpty(_projectName))
                _eventAggregator.GetEvent<AppGameUpdateFilesEvent>().Publish(new ProcessFilesUpdateGameMode()
                {
                    NameMode = _projectName,
                    UpdateCountFilesHandler = e
                });
        }

        private void UpdateUnpackStatsHandle(object sender, UnpackHandle e)
        {
            if (!string.IsNullOrEmpty(_projectName))
                _eventAggregator.GetEvent<AppGameUnpackFilesEvent>().Publish(new ProcessUnpackFilesGamesModel()
                {
                    NameMode = _projectName,
                    UpdateCountFilesHandler = e
                });
        }
    }
}
