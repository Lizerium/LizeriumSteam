/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 29 сентября 2026 09:46:29
 * Version: 1.0.186
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