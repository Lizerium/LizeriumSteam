/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 03 мая 2026 07:12:30
 * Version: 1.0.40
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
