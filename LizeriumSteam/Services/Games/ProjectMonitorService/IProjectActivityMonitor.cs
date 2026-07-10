/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 10 июля 2026 12:17:08
 * Version: 1.0.106
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
