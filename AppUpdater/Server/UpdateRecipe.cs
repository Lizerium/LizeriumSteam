/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 29 апреля 2026 07:13:25
 * Version: 1.0.36
 */

using System.Collections.Generic;

namespace AppUpdater.Recipe
{
    public class UpdateRecipe
    {
        public string NewVersion { get; private set; }
        public string CurrentVersion { get; private set; }
        public IEnumerable<UpdateRecipeFile> Files { get; private set; }

        public UpdateRecipe(string newVersion, string currentVersion, IEnumerable<UpdateRecipeFile> files)
        {
            this.NewVersion = newVersion;
            this.CurrentVersion = currentVersion;
            this.Files = files;
        }
    }
}
