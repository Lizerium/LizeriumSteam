/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 11 мая 2026 10:58:07
 * Version: 1.0.48
 */

namespace LizeriumSteam.Services.Games.GameManifestMonitorService
{
    public interface IGameManifestMonitorService
    {
        void StartMonitoring();
        void StopMonitoring();
    }
}
