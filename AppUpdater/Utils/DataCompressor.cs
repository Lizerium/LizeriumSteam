/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 18 апреля 2026 15:00:38
 * Version: 1.0.25
 */

using System.IO.Compression;
using System.IO;

namespace AppUpdater.Utils
{
    public class DataCompressor
    {
        public static void Compress(Stream inputStream, Stream outputStream)
        {
            using (GZipStream zip = new GZipStream(outputStream, CompressionMode.Compress))
            {
                inputStream.CopyTo(zip);
            }
        }

        public static byte[] Compress(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            using (MemoryStream msInput = new MemoryStream())
            {
                msInput.Write(data, 0, data.Length);
                msInput.Position = 0;

                using (MemoryStream msOutput = new MemoryStream())
                {
                    Compress(msInput, msOutput);

                    return msOutput.ToArray();
                }
            }
        }

        public static void Decompress(Stream inputStream, Stream outputStream)
        {
            using (GZipStream zip = new GZipStream(inputStream, CompressionMode.Decompress))
            {
                zip.CopyTo(outputStream);
            }
        }

        public static byte[] Decompress(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            using (MemoryStream msInput = new MemoryStream())
            {
                msInput.Write(data, 0, data.Length);
                msInput.Position = 0;

                using (MemoryStream msOutput = new MemoryStream())
                {
                    Decompress(msInput, msOutput);

                    return msOutput.ToArray();
                }
            }
        }
    }
}
