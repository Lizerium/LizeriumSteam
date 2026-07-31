/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 31 июля 2026 16:48:44
 * Version: 1.0.127
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
