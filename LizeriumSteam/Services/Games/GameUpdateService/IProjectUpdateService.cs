/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 12 мая 2026 12:23:07
 * Version: 1.0.49
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
