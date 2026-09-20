/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 20 сентября 2026 08:50:48
 * Version: 1.0.177
 */


namespace AppUpdater
{
    public class UpdateInfo
    {
        public string Version { get; private set; }
        public bool HasUpdate { get; private set; }
        public bool NotAwailableServer { get; set; }

        public UpdateInfo(bool hasUpdate, string version)
        {
            this.HasUpdate = hasUpdate;
            this.Version = version;
            NotAwailableServer = false;
        }
    }
}
