/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 05 июля 2026 11:14:27
 * Version: 1.0.101
 */

namespace LizeriumSteam.Services.Update
{
    public interface IAppUpdateService
    {
        void CheckConfigurationAndCompare();
        void InitializeAutoUpdate();
        void StopAutoUpdate();
    }
}
