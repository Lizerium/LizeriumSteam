/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 13 апреля 2026 13:12:10
 * Version: 1.0.18
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
