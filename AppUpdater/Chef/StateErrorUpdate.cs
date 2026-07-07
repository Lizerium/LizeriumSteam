/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 07 июля 2026 12:42:05
 * Version: 1.0.103
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
