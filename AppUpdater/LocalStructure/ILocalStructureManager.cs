/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 31 марта 2026 11:07:39
 * Version: 1.0.4
 */

using AppUpdater.LocalStructure.Components;
using AppUpdater.Manifest;
using System;

namespace AppUpdater.LocalStructure
{
    public interface ILocalStructureManager
    {
        string GetBaseDir();
        void CreateVersionDir(string version);
        void DeleteVersionDir(string version);
        string[] GetInstalledVersions();
        VersionManifest LoadManifest(string version);
        string GetCurrentVersion();
        void SetCurrentVersion(string version);
        string GetLastValidVersion();
        void SetLastValidVersion(string version);
        string GetExecutingVersion();
        bool HasVersionFolder(string version);
        void CopyFile(string originVersion, string destinationVersion, string filename);
        StateSaveFile SaveFile(string version, string filename, byte[] data);
        void ApplyDelta(string originalVersion, string newVersion, string filename, byte[] deltaData);
        Uri GetUpdateServerUri();
        ResultMergeConfigs CompareConfigs(StateSaveFile state);
    }
}
