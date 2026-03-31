/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 31 марта 2026 11:07:39
 * Version: 1.0.4
 */

namespace LizeriumSteam.Services.Games.GameManifestMonitorService
{
    public interface IGameManifestMonitorService
    {
        void StartMonitoring();
        void StopMonitoring();
    }
}
