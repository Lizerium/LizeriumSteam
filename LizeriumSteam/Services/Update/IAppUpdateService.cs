/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 12 июня 2026 07:13:43
 * Version: 1.0.79
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
