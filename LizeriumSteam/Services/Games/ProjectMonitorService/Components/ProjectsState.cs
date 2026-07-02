/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 02 июля 2026 07:34:24
 * Version: 1.0.98
 */

using System;

namespace LizeriumSteam.Services.Games.ProjectMonitorService.Components
{
    public class ProjectsState
    {  
        /// <summary>
        /// Имя последнего запущенного проекта
        /// </summary>
        public string LastActiveProject { get; set; }
        /// <summary>
        /// Время последнего запуска
        /// </summary>
        public DateTime LastRunTime { get; set; } = DateTime.Now;
    }
}
