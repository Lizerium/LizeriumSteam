/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 08 апреля 2026 14:44:03
 * Version: 1.0.13
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
