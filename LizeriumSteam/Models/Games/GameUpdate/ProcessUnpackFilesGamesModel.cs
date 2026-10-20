/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 20 октября 2026 06:58:56
 * Version: 1.0.206
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
