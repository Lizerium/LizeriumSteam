/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 09 мая 2026 08:13:20
 * Version: 1.0.46
 */

namespace AppUpdater.Chef
{
    public enum StateErrorUpdate
    {
        Wait,
        ErrorSaveFile,
        ErrorUnpackFile,
        ErrorConnectServer,
        Success,
        CancelUpdate,
        NotAvailableUpdate
    }
}
