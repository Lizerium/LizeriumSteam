/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 05 сентября 2026 08:57:39
 * Version: 1.0.162
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
