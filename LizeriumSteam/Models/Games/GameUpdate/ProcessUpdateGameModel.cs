/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 07 июня 2026 18:42:45
 * Version: 1.0.74
 */

using AppUpdater.Chef;
using AppUpdater.Server;

namespace LizeriumSteam.Models.Games.GameUpdate
{
    public class ProcessUpdateGameModel
    {
        public string NameMode { get; set; }
        public string TitleView { get; set; }
        public DataDownloadHandle Progress { get; set; }
        public string NewVersion { get; internal set; }
        public string OldVersion { get; internal set; }
        public StateErrorUpdate State { get; internal set; }
    }
}
