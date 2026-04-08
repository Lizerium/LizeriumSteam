/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 08 апреля 2026 14:44:03
 * Version: 1.0.13
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
