/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 02 августа 2026 07:14:12
 * Version: 1.0.129
 */

namespace LizeriumSteam.Services.Games.GameManifestMonitorService
{
    public interface IGameManifestMonitorService
    {
        void StartMonitoring();
        void StopMonitoring();
    }
}
