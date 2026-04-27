/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 27 апреля 2026 10:02:12
 * Version: 1.0.34
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
