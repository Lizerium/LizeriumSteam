/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 14 июня 2026 16:53:40
 * Version: 1.0.81
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace AppUpdater.Utils
{
    public static class Checksum
    {
        public static string Calculate(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            SHA256Managed sha = new SHA256Managed();
            byte[] checksum = sha.ComputeHash(stream);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in checksum)
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
