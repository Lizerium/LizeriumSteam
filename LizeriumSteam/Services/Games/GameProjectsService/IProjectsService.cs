/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 08 сентября 2026 06:57:34
 * Version: 1.0.165
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