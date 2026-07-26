/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 26 июля 2026 15:33:11
 * Version: 1.0.122
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
