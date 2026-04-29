/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 29 апреля 2026 07:13:25
 * Version: 1.0.36
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