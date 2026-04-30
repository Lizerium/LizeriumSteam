/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 30 апреля 2026 09:40:03
 * Version: 1.0.37
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
