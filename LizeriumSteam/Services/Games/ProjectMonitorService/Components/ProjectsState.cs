/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 22 апреля 2026 18:58:37
 * Version: 1.0.29
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
