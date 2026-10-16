/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 16 октября 2026 08:55:33
 * Version: 1.0.202
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
