/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 04 мая 2026 07:13:51
 * Version: 1.0.41
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
