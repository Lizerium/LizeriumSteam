/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 08 мая 2026 07:07:38
 * Version: 1.0.45
 */

using System.Collections.Generic;

namespace AppUpdater.Manifest
{
    public class UpdateManifestModel
    {
        public List<string> AddedDirectory { get; set; } = new List<string>();
        public List<string> AddedFiles { get; set; } = new List<string>();
        public List<string> ChangedFiles { get; set; } = new List<string>();
        public List<string> DeletedFiles { get; set; } = new List<string>();
        public List<string> DeletedDirectory { get; set; } = new List<string>();
        public List<string> RetranslateFiles { get; set; } = new List<string>();
        public List<string> RetranslateDirectory { get; set; } = new List<string>();
    }
}
