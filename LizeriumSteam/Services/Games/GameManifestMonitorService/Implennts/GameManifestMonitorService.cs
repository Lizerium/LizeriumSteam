/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 03 апреля 2026 11:47:22
 * Version: 1.0.8
 */

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using LizeriumSteam.Events;
using LizeriumSteam.Models.Games;
using LizeriumSteam.Services.Settings;

using Microsoft.Extensions.Logging;

using Prism.Events;

namespace LizeriumSteam.Services.Games.GameManifestMonitorService.Implennts
{
    /// <summary>
    /// Мониторит конфигурацию проекта и обновляет параметры настроек в реальном времени
    /// </summary>
    public class GameManifestMonitorService : IGameManifestMonitorService, IDisposable
    {
        private readonly IAppSettings _appSettings;
        private readonly ILogger<GameManifestMonitorService> _logger;
        private readonly IEventAggregator _eventAggregator;
        private CancellationTokenSource _cts;
        private Task _monitoringTask;

        private readonly TimeSpan _interval = TimeSpan.FromSeconds(10); // интервал проверки

        public GameManifestMonitorService(IAppSettings appSettings,
            ILogger<GameManifestMonitorService> logger,
            IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _appSettings = appSettings;
            _logger = logger;

            // подписываемся на событие
            _eventAggregator.GetEvent<StopManifestMonitorEvent>().Subscribe(StopMonitoring);
            _eventAggregator.GetEvent<StartManifestMonitorEvent>().Subscribe(StartMonitoring);
        }

        /// <summary>
        /// Запуск фонового мониторинга
        /// </summary>
        public void StartMonitoring()
        {
            if (_cts != null && !_cts.IsCancellationRequested)
                return; // уже запущено

            _cts = new CancellationTokenSource();
            _monitoringTask = Task.Run(() => MonitorLoopAsync(_cts.Token));
        }

        private async Task MonitorLoopAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        MonitorGames();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Ошибка во время мониторинга манифестов");
                    }

                    await Task.Delay(_interval, token); // ждём следующий тик
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Фоновый мониторинг остановлен.");
            }
        }

        private void MonitorGames()
        {
            var games = _appSettings.GameButtonInfo?.ToList(); // копия для потокобезопасности
            if (games == null || games.Count == 0)
                return;

            foreach (var game in games)
            {
                FixedLastVersion(game);
                FixedExistChangesHistory(game);
                ExistUpdateFile(game);
            }
        }

        private void ExistUpdateFile(GameButtonModel game)
        {
            game.ExistFileInfoUpdatesPath = File.Exists(game.FileInfoUpdatesPath);
        }

        /// <summary>
        /// Исправление несоответствия версий
        /// </summary>
        private void FixedLastVersion(GameButtonModel game)
        {
            var pathGameConfig = Path.Combine(game.GameFolderFullPath, "manifest.launcher");
            if (!File.Exists(pathGameConfig))
            {
                game.LastVersionInstall = "🚧";
                return;
            }

            // ждём пока файл реально освободится
            for (int i = 0; i < 5; i++)
            {
                if (IsFileReady(pathGameConfig))
                    break;

                _logger.LogInformation("Файл {0} занят, жду...", pathGameConfig);
                Thread.Sleep(500); // полсекунды
            }

            XDocument xdoc = null;

            // Несколько попыток, если файл занят другим процессом
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    using (var fs = new FileStream(
                        pathGameConfig,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.ReadWrite)) // <--- главное отличие
                    {
                        xdoc = XDocument.Load(fs);
                    }
                    break; // если удалось загрузить, выходим из цикла
                }
                catch (IOException)
                {
                    _logger.LogWarning("Файл {0} занят, попытка {1}/3...", pathGameConfig, i + 1);
                    Thread.Sleep(200); // ждём и пробуем снова
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка чтения {0}", pathGameConfig);
                    return;
                }
            }

            if (xdoc == null)
                return;

            var versionProject = xdoc.Root != null ? xdoc.Root.Element("version")?.Value : null;
            if (string.IsNullOrEmpty(versionProject))
                return;

            if (game.LastVersionInstall != versionProject)
            {
                var oldVersion = game.LastVersionInstall;
                game.LastVersionInstall = versionProject;

                _appSettings.ChangePanelsConfigurationKey(
                    string.Format("mods/{0}/LastVersionInstall", game.NameMode),
                    versionProject);

                _logger.LogWarning(
                    "Несоответствие версии игры: {0} -> {1}. Конфигурация исправлена.",
                    oldVersion, versionProject);
            }
        }

        /// <summary>
        /// Проверка доступности манифеста в системе
        /// </summary>
        /// <param name="path">Путь до манифеста</param>
        /// <returns>bool</returns>
        private bool IsFileReady(string path)
        {
            try
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return stream.Length > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Проверка и обновление существования файла истории изменений
        /// </summary>
        /// <param name="game">game</param>
        private void FixedExistChangesHistory(GameButtonModel game)
        {
            var pathGameConfig = Path.Combine(game.GameFolderFullPath, "updates_info.json");
            if (File.Exists(pathGameConfig))
            {
                game.AvailableChangesHistory = true;
                return;
            }
            else game.AvailableChangesHistory = false;
        }

        public void StopMonitoring()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        public void Dispose()
        {
            StopMonitoring();
            _cts?.Dispose();
        }
    }
}
