/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 28 апреля 2026 14:44:28
 * Version: 1.0.35
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
