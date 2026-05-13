/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 13 мая 2026 13:35:52
 * Version: 1.0.50
 */

using System;
using System.Threading;
using System.Threading.Tasks;

using AppUpdater.Chef;
using AppUpdater.Log;

namespace AppUpdater
{
    public class AutoUpdater
    {
        private readonly ILog log = Logger.For<AutoUpdater>();
        private int secondsBetweenChecks;
        private readonly IUpdateManager updateManager;
        private Thread thread;
        private CancellationTokenSource updateCancellationTokenSource;

        public int SecondsBetweenChecks
        {
            get { return secondsBetweenChecks; }
            set { secondsBetweenChecks = value; }
        }

        public event EventHandler<double> PercentUpdate;
        public event EventHandler<StateErrorUpdate> Updated;
        public event EventHandler<string> EnableUpdated;

        public AutoUpdater(IUpdateManager updateManager)
        {
            updateCancellationTokenSource = new CancellationTokenSource();
            this.updateManager = updateManager;
            secondsBetweenChecks = 3600;
        }

        public void Start()
        {
            if (thread == null || !thread.IsAlive)
            {
                log.Debug("Starting the AutoUpdater.");
                thread = new Thread(new ThreadStart(() => CheckForUpdates(updateCancellationTokenSource.Token)));
                thread.IsBackground = true;
                thread.Start();
            }
        }

        public void Stop()
        {
            if (updateCancellationTokenSource != null)
            {
                log.Debug("Stopping the AutoUpdater.");
                updateCancellationTokenSource.Cancel(); // отменяем токен
            }
        }

        private async void CheckForUpdates(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    log.Debug("Checking for updates.");
                    UpdateInfo updateInfo = await updateManager.CheckForUpdate();
                    if(updateInfo.NotAwailableServer)
                        RaiseUpdated(StateErrorUpdate.ErrorConnectServer);
                    if (updateInfo != null && updateInfo.HasUpdate)
                    {
                        EnableUpdated?.Invoke(this, updateInfo.Version);
                        updateManager.PercentUpdate = null;
                        updateManager.PercentUpdate += UpdatePercent;
                        log.Debug("Updates found. Installing new files.");
                        await updateManager.DoUpdate(updateInfo, token);
                        log.Debug("Update is ready.");
                        RaiseUpdated(updateManager.StateCook);
                    }
                    else
                    {
                        RaiseUpdated(StateErrorUpdate.NotAvailableUpdate);
                        log.Debug("No updates found.");
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }

                try
                {
                    await Task.Delay(secondsBetweenChecks * 1000, token);
                }
                catch (OperationCanceledException)
                {
                    // Отмена Task.Delay при остановке
                    log.Debug("Delay cancelled due to shutdown.");
                    break;
                }
            }
        }

        private void UpdatePercent(object sender, double percent)
        {
            PercentUpdate?.Invoke(this, percent);
        }

        private void RaiseUpdated(StateErrorUpdate stateErrorUpdate)
        {
            Updated?.Invoke(this, stateErrorUpdate);
        }
    }
}
