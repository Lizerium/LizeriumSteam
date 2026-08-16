/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 16 августа 2026 14:47:00
 * Version: 1.0.143
 */

using System.Linq;
using System.Collections.Generic;
namespace AppUpdater.Manifest
{
    public class VersionManifestFile
    {
        public string Name { get; private set; }
        public string Checksum { get; private set; }
        public long Size { get; private set; }
        public List<VersionManifestDeltaFile> Deltas { get; private set; }

        public string DeployedName
        {
            get
            {
                return Name == null ? null : Name + ".deploy";
            }
        }

        public VersionManifestFile(string name, string checksum, long size, IEnumerable<VersionManifestDeltaFile> deltas = null)
        {
            this.Name = name;
            this.Checksum = checksum;
            this.Size = size;
            this.Deltas = new List<VersionManifestDeltaFile>(deltas ?? new VersionManifestDeltaFile[0]);
        }

        public VersionManifestDeltaFile GetDeltaFrom(string checksum)
        {
            return Deltas.FirstOrDefault(x => x.Checksum == checksum);
        }
    }
}
