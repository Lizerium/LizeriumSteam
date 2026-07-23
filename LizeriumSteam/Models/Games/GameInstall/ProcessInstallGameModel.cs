/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 23 июля 2026 13:16:32
 * Version: 1.0.119
 */

namespace LizeriumSteam.Models.Games.GameInstall
{
    public class ProcessInstallGameModel
    {
        public GameButtonModel GameButtonModel { get; set; }
        public string CurrentStartupProjectFolder { get; set; }
        public string CurrentStartupFileInstaller { get; internal set; }
    }
}
