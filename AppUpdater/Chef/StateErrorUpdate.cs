/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 12 августа 2026 07:13:58
 * Version: 1.0.139
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
