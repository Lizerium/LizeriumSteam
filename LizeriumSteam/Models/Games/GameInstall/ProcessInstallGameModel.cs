/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 20 июня 2026 07:10:40
 * Version: 1.0.87
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
