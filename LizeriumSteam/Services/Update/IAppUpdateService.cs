/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 22 сентября 2026 09:24:01
 * Version: 1.0.179
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
