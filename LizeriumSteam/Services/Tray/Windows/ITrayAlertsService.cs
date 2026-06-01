/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 01 июня 2026 15:07:31
 * Version: 1.0.68
 */

using AppUpdater.Chef;

using LizeriumSteam.Models.Games;
using LizeriumSteam.Models.Games.GameUpdate;

namespace LizeriumSteam.Services.Tray.Windows
{
    public interface ITrayAlertsService
    {
        void ShowTrayCompleteUpdate(StateErrorUpdate state);
        void ShowTrayUpdateAvailable(string version);
        void ShowTrayCompleteUpdateProject(ProcessUpdateGameModel update);
        void ShowTrayCancelUpdateProject(ProcessUpdateGameModel process);
        void ShowTrayCompleteDownloadProject(ProcessUpdateGameModel process);
        void ShowTrayCompleteInstallProject(GameButtonModel gameButtonModel);
        void ShowTrayCancelDownloadProject(ProcessUpdateGameModel process);
        void ShowTrayNotAvailableUpdateProject(ProcessUpdateGameModel process);
        void ShowTrayNotCompleteInstallProject(GameButtonModel gameButtonModel);
    }
}