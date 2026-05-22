/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 22 мая 2026 12:01:24
 * Version: 1.0.58
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
