/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 20 августа 2026 09:58:48
 * Version: 1.0.147
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
