/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 07 августа 2026 08:45:50
 * Version: 1.0.134
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
