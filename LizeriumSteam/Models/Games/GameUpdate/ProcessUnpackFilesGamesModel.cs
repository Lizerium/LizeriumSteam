/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 09 июля 2026 07:23:31
 * Version: 1.0.105
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
