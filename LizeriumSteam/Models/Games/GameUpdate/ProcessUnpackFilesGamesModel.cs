/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 19 мая 2026 10:36:38
 * Version: 1.0.55
 */

using AppUpdater.Server;

namespace LizeriumSteam.Models.Games.GameUpdate
{
    public class ProcessUnpackFilesGamesModel
    {
        public string NameMode { get; set; }
        public UnpackHandle UpdateCountFilesHandler { get; set; }
    }
}
