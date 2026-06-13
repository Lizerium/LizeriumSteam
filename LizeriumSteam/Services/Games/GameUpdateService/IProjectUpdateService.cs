/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 13 июня 2026 14:20:15
 * Version: 1.0.80
 */

using System.Threading.Tasks;

using LizeriumSteam.Models.Games;

namespace LizeriumSteam.Services.Games.GameUpdateService
{
    public interface IProjectUpdateService
    {
        /// <summary>
        /// Установка первичной версии приложения
        /// 
        /// Папка установки
        /// C:\Users\<User>\AppData\Local\<Project>
        /// </summary>
        void InstallProject(GameButtonModel gameButtonModel);

        /// <summary>
        /// Остановка обновления проекта
        /// </summary>
        /// <param name="game">Данные проекта</param>
        void StopUpdateProject(GameButtonModel game);

        /// <summary>
        /// Обновление проекта
        /// </summary>
        /// <param name="game">Данные проекта</param>
        Task UpdateProject(GameButtonModel game);
    }
}
