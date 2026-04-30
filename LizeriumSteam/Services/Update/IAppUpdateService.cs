/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 30 апреля 2026 09:40:03
 * Version: 1.0.37
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
