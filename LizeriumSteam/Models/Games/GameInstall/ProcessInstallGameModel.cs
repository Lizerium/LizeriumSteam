/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 28 сентября 2026 12:14:44
 * Version: 1.0.185
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
