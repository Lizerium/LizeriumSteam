/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 05 мая 2026 07:22:03
 * Version: 1.0.42
 */

namespace LizeriumSteam.Services.Games.GameManifestMonitorService
{
    public interface IGameManifestMonitorService
    {
        void StartMonitoring();
        void StopMonitoring();
    }
}
