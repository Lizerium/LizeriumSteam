/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 02 мая 2026 19:36:41
 * Version: 1.0.39
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
