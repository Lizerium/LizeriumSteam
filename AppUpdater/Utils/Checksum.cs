/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 03 мая 2026 07:12:30
 * Version: 1.0.40
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
