/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 24 июля 2026 11:59:49
 * Version: 1.0.120
 */

using System;
using System.Collections.Generic;

using LizeriumSteam.Services.Games.ProjectMonitorService.Components;

namespace LizeriumSteam.Services.Games.ProjectMonitorService
{
    public interface IProjectActivityMonitor
    {
        void StartMonitoring();
        void StopMonitoring();
        List<ProjectsState> GetLastActiveProject();
        event Action<List<ProjectsState>> ActiveProjectChanged;
    }
}
