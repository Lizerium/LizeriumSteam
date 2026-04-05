/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 05 апреля 2026 15:18:02
 * Version: 1.0.10
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
