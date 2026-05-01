/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 01 мая 2026 07:14:08
 * Version: 1.0.38
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AppUpdater.Utils
{
    public static class PathUtils
    {
        public static string AddTrailingSlash(string path)
        {
            if (path == null || path.Length == 0)
            {
                return path;
            }

            if (path[path.Length - 1] != Path.DirectorySeparatorChar)
            {
                return path + Path.DirectorySeparatorChar;
            }
            else
            {
                return path;
            }
        }
    }
}
