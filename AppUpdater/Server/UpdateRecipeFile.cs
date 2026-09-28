/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 28 сентября 2026 12:14:44
 * Version: 1.0.185
 */


namespace AppUpdater.Recipe
{
    public enum FileUpdateAction
    {
        Copy,
        Download,
        DownloadDelta
    }

    public class UpdateRecipeFile
    {
        public string Name { get; private set; }
        public string Checksum { get; private set; }
        public long Size { get; private set; }
        public FileUpdateAction Action { get; private set; }
        public string FileToDownload { get; private set; }

        public UpdateRecipeFile(string name, string checksum, long size, FileUpdateAction action, string fileToDownload)
        {
            this.Name = name;
            this.Checksum = checksum;
            this.Size = size;
            this.Action = action;
            this.FileToDownload = fileToDownload;
        }
    }
}
