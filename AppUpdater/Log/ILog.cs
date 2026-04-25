/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 25 апреля 2026 08:31:42
 * Version: 1.0.32
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppUpdater.Log
{
    public interface ILog
    {
        void Info(string message, params object[] values);
        void Warn(string message, params object[] values);
        void Error(string message, params object[] values);
        void Debug(string message, params object[] values);
    }
}
