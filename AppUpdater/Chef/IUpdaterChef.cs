/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 08 сентября 2026 06:57:34
 * Version: 1.0.165
 */

using AppUpdater.Recipe;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AppUpdater.Chef
{
    public interface IUpdaterChef
    {
        Task<StateErrorUpdate> Cook(UpdateRecipe recipe, CancellationToken token);
        void CookMode(CancellationToken token, UpdateRecipe recipe, string Mode);
        void EndCookEvent(StateErrorUpdate cancelUpdate);
        EventHandler<double> PercentUpdate { get; set; }
        EventHandler<StateErrorUpdate> EndCook { get; set; }
        EventHandler<UpdateCountFilesHandler> UpdateCountFilesHandler { get; set; }
        EventHandler<StateErrorUpdate> ErrorUpdateLauncher { get; set; }
    }
}
