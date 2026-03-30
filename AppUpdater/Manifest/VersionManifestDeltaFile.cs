/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 30 марта 2026 12:35:16
 * Version: 1.0.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppUpdater.Manifest
{
    public class VersionManifestDeltaFile
    {
        public string Filename { get; private set; }
        public string Checksum { get; private set; }
        public long Size { get; private set; }

        public VersionManifestDeltaFile(string filename, string checksum, long size)
        {
            this.Filename = filename;
            this.Checksum = checksum;
            this.Size = size;
        }
    }
}
