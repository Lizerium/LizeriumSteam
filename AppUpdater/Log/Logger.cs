/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 13 мая 2026 13:35:52
 * Version: 1.0.50
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppUpdater.Log
{
    public class Logger
    {
        public static Func<Type, ILog> LoggerProvider { get; set; }

        static Logger()
        {
            LoggerProvider = DefaultLoggerProvider;
        }

        public static ILog For(Type type)
        {
            try
            {
                return LoggerProvider(type);
            }
            catch
            {
                return EmptyLog.Instance;
            }
        }

        public static ILog For<T>()
        {
            return For(typeof(T));
        }

        private static ILog DefaultLoggerProvider(Type type)
        {
            return EmptyLog.Instance;
        }
    }
}
