/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 21 мая 2026 11:58:51
 * Version: 1.0.57
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
