/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 25 апреля 2026 08:31:42
 * Version: 1.0.32
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using LizeriumSteam.Events;
using LizeriumSteam.Services.Games.ProjectMonitorService.Components;
using LizeriumSteam.Services.Settings;

using Microsoft.Extensions.Logging;

using Prism.Events;

namespace LizeriumSteam.Services.Games.ProjectMonitorService.Implennts
{
    /// <summary>
    /// Cервис мониторинга активных проектов 
    /// </summary>
    public class ProjectActivityMonitorService : IProjectActivityMonitor
    {
        /// <summary>
        /// Система логирования
        /// </summary>
        private readonly ILogger<ProjectActivityMonitorService> _logger;
        private readonly IAppSettings _appSettings;
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// Папка для хранения файла последнего сохранения состояни активного процесса игры
        /// </summary>
        private string _stateFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LizeriumLauncher");
        /// <summary>
        /// Файл который хранит имя последнего активного проекта
        /// </summary>
        private string _stateFilePath => Path.Combine(_stateFolder, "last_active_project.json");
        /// <summary>
        /// Состояние последнего активного проекта в системе
        /// </summary>
        public event Action<List<ProjectsState>> ActiveProjectChanged;

        private readonly Dictionary<string, string> startupProcessesFullPath = new Dictionary<string, string>();
        private List<string> lastRecordedProjects = new List<string>();
        private CancellationTokenSource _cts;

        public ProjectActivityMonitorService(ILogger<ProjectActivityMonitorService> logger,
            IAppSettings appSettings,
            IEventAggregator eventAggregator)
        {
            _logger = logger;
            _appSettings = appSettings;
            _eventAggregator = eventAggregator;

            // подписываемся на событие готовности
            _eventAggregator.GetEvent<AppReadyEvent>()
                .Subscribe(OnAppReady, ThreadOption.UIThread);
        }

        private void OnAppReady()
        {
            StartMonitoring();
        }

        public void StartMonitoring()
        {
            try
            {
                _cts = new CancellationTokenSource();
                LoadStartupProcesses();

                Task.Run(async () =>
                {
                    while (!_cts.IsCancellationRequested)
                    {
                        var activeProjects = GetActiveProjectFromProcesses();
                        // Проверяем, изменилось ли состояние проектов
                        if (!Enumerable.SequenceEqual(activeProjects, lastRecordedProjects))
                        {
                            SaveLastActiveProject(activeProjects);
                            lastRecordedProjects = new List<string>(activeProjects);

                            _logger.LogInformation($"[ProjectActivityMonitorService] Активные проекты: {string.Join(", ", activeProjects)}");
                        }
                        await Task.Delay(2000, _cts.Token);
                    }
                }, _cts.Token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ProjectActivityMonitorService] ERROR: {ex.Message} | {ex.StackTrace}");
            }
        }

        public void StopMonitoring()
        {
            _cts?.Cancel();
        }

        private void LoadStartupProcesses()
        {
            if (_appSettings.GameButtonInfo != null)
            {
                foreach(var game in _appSettings.GameButtonInfo)
                {
                    if (string.IsNullOrWhiteSpace(game.GameFolderFullPath)
                        || string.IsNullOrWhiteSpace(game.StartupFile))
                        continue;

                    string fullPath = Path.Combine(game.GameFolderFullPath, game.StartupFolder ?? "", game.StartupFile);
                    fullPath = Path.GetFullPath(fullPath);

                    if(File.Exists(fullPath))
                        startupProcessesFullPath[game.NameMode] = fullPath;
                }
            }
        }

        private List<string> GetActiveProjectFromProcesses()
        {
            var activeProjects = new List<string>();

            foreach (var proc in Process.GetProcesses())
            {
                try
                {
                    var contains = startupProcessesFullPath.Select(it => it.Value).Where(it => it.Contains(proc.ProcessName + ".exe")).ToList();
                    if (contains.Count <= 0)
                        continue;

                    string exePath = proc.MainModule?.FileName;
                    if (string.IsNullOrWhiteSpace(exePath))
                        continue;

                    foreach (var pair in startupProcessesFullPath)
                    {
                        if (exePath.Equals(pair.Value, StringComparison.OrdinalIgnoreCase))
                        {
                            if (!activeProjects.Contains(pair.Key))
                                activeProjects.Add(pair.Key);
                        }
                    }
                }
                catch (Exception ex)
                {  // Некоторые системные процессы не дают доступ к MainModule — игнорируем
                    _logger.LogError($"[ProjectActivityMonitorService] ERROR: {ex.Message} | {ex.StackTrace}");
                }
            }
            return activeProjects;
        }

        private void SaveLastActiveProject(List<string> projects)
        {
            var states = new List<ProjectsState>();
            if (projects.Count > 0)
            {
                foreach (var projectName in projects)
                {
                    var state = new ProjectsState
                    {
                        LastActiveProject = projectName,
                        LastRunTime = DateTime.Now,
                    };
                    states.Add(state);
                }


                var folder = Path.GetDirectoryName(_stateFilePath);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                File.WriteAllText(_stateFilePath, JsonSerializer.Serialize(states));
            }
            ActiveProjectChanged?.Invoke(states);
        }

        public List<ProjectsState> GetLastActiveProject()
        {
            if (File.Exists(_stateFilePath))
            {
                try
                {
                    string json = File.ReadAllText(_stateFilePath);
                    var data = JsonSerializer.Deserialize<List<ProjectsState>>(json);
                    return data;
                }
                catch (Exception ex)
                {
                    // Логируем ошибку
                    _logger.LogError(ex, "Ошибка при чтении файла состояния {File}", _stateFilePath);

                    try
                    {
                        File.Delete(_stateFilePath);
                        _logger.LogWarning("Файл состояния удалён: {File}", _stateFilePath);
                    }
                    catch (Exception delEx)
                    {
                        _logger.LogError(delEx, "Не удалось удалить повреждённый файл {File}", _stateFilePath);
                    }

                    return null;
                }
            }

            return null;
        }
    }
}
