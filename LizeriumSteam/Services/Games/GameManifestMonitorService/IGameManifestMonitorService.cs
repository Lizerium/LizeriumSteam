/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 14 июля 2026 07:17:45
 * Version: 1.0.110
 */

namespace LizeriumSteam.Services.Games.GameManifestMonitorService
{
    public interface IGameManifestMonitorService
    {
        void StartMonitoring();
        void StopMonitoring();
    }
}
