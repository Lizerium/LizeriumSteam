/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 08 октября 2026 06:57:57
 * Version: 1.0.195
 */

using System.Collections.Generic;

namespace AppUpdater.LocalStructure.Components
{
    public class ResultMergeConfigs
    {
        public List<MergeConfigKeyValues> Result { get; set; } = new List<MergeConfigKeyValues>();
    }

    public class MergeConfigKeyValues
    {
        public string ModName { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return $"Add key::{Key} and value::{Value} to Mod::{ModName}";
        }
    }
}
