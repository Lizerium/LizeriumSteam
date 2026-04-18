/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 18 апреля 2026 15:00:38
 * Version: 1.0.25
 */

using System.Collections.ObjectModel;
using System.Threading.Tasks;

using LizeriumSteam.Models.Games;

namespace LizeriumSteam.Services.Games.GameProjectsService
{
    public interface IProjectsService
    {
        /// <summary>
        /// Инициализация проектов
        /// </summary>
        Task Initialize();
        /// <summary>
        /// Провка и фиксация состояний проекта
        /// </summary>
        /// <param name="data">Модель данных проекта</param>
        /// <returns>StateMode</returns>
        Task<StateMode> CheckAndSetupStatesProject(GameButtonModel data);
        ObservableCollection<GameButtonModel> GameButtons { get; }
    }
}