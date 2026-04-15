/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 15 апреля 2026 07:05:07
 * Version: 1.0.22
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
