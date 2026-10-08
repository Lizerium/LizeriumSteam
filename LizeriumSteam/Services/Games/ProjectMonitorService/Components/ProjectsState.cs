/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 08 октября 2026 06:57:57
 * Version: 1.0.195
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
