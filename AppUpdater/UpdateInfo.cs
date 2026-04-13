/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 13 апреля 2026 13:12:10
 * Version: 1.0.18
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
