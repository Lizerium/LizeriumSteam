/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 08 октября 2026 06:57:57
 * Version: 1.0.195
 */


using AppUpdater.Chef;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AppUpdater
{
    public interface IUpdateManager
    {
        void Initialize();
        Task<UpdateInfo> CheckForUpdate();
        Task DoUpdate(UpdateInfo updateInfo, CancellationToken token);
        EventHandler<double> PercentUpdate { get; set; }
        EventHandler<StateErrorUpdate> EndCookHandle { get; set; }
        EventHandler<UpdateCountFilesHandler> UpdateCountFilesHandler { get; set; }
        StateErrorUpdate StateCook { get; set; }
    }
}
