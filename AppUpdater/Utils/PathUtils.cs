/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 20 августа 2026 09:58:48
 * Version: 1.0.147
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
