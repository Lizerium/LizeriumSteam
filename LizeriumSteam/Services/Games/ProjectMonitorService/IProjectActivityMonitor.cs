/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 15 июня 2026 07:13:07
 * Version: 1.0.82
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
