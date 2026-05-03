/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 03 мая 2026 07:12:30
 * Version: 1.0.40
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using AppUpdater.Delta;
using AppUpdater.LocalStructure.Components;
using AppUpdater.Log;
using AppUpdater.Manifest;
using AppUpdater.Utils;

namespace AppUpdater.LocalStructure
{
    public class DefaultLocalStructureManager : ILocalStructureManager
    {
        public string baseDir;
        public static Func<string> GetExecutablePath = GetExecutingAssemblyLocation;
        private readonly ILog log = Logger.For<AutoUpdater>();

        public DefaultLocalStructureManager(string baseDir)
        {
            this.baseDir = baseDir;
        }

        public void CreateVersionDir(string version)
        {
            Directory.CreateDirectory(GetVersionDir(version));
        }

        public void DeleteVersionDir(string version)
        {
            Directory.Delete(GetVersionDir(version), true);
        }

        public string[] GetInstalledVersions()
        {
            string baseDirectory = PathUtils.AddTrailingSlash(baseDir);

            return Directory.EnumerateDirectories(baseDirectory)
                            .Select(x => x.Remove(0, baseDirectory.Length))
                            .ToArray();
        }

        public VersionManifest LoadManifest(string version)
        {
            string versionDir = GetVersionDir(version);
            return VersionManifest.GenerateFromDirectory(version, versionDir);
        }

        public string GetCurrentVersion()
        {
            return GetConfigValue("version");
        }

        public void SetCurrentVersion(string version)
        {
            SetConfigValue("version", version);
        }

        public string GetBaseDir()
        {
            return baseDir;
        }

        public ResultMergeConfigs CompareConfigs(StateSaveFile state)
        {
            if(string.IsNullOrEmpty(state.PathFile)) return new ResultMergeConfigs();
            if (!File.Exists(state.PathFile)) return new ResultMergeConfigs();

            return MergeNewKeysFromConfig(state.PathFile);
        }

        public string GetLastValidVersion()
        {
            return GetConfigValue("last_version");
        }

        public void SetLastValidVersion(string version)
        {
            SetConfigValue("last_version", version);
        }

        public string GetExecutingVersion()
        {
            return Directory.GetParent(GetExecutablePath()).Name;
        }

        public bool HasVersionFolder(string version)
        {
            return Directory.Exists(GetVersionDir(version));
        }

        public void CopyFile(string originVersion, string destinationVersion, string filename)
        {
            string originFilename = Path.Combine(GetVersionDir(originVersion), filename);
            string destinationFilename = Path.Combine(GetVersionDir(destinationVersion), filename);

            File.Copy(originFilename, destinationFilename, true);
        }

        public StateSaveFile SaveFile(string version, string filename, byte[] data)
        {
            try
            {
                string destinationFilename = Path.Combine(GetVersionDir(version), filename);
                if (!Directory.Exists(Path.GetDirectoryName(destinationFilename)))
                {
                    var index_1 = filename.IndexOf('\\') + 1;
                    var lenght = filename.Length - filename.IndexOf('\\') - 1;
                    var newName = filename.Substring(index_1, lenght);
                    destinationFilename = Path.Combine(GetVersionDir(version), newName);
                }

                if (!Directory.Exists(Path.GetDirectoryName(destinationFilename)))
                {
                    log.Error($"{destinationFilename} не существует такой директории! Создаю...");
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationFilename));
                }

                File.WriteAllBytes(destinationFilename, data);
                return new StateSaveFile()
                {
                    NameFile = filename,
                    PathFile = destinationFilename,
                    State = true
                };
            }
            catch
            {
                return new StateSaveFile()
                {
                    NameFile = filename,
                    State = false
                };
            }
        }

        public void ApplyDelta(string originalVersion, string newVersion, string filename, byte[] deltaData)
        {
            string originalFile = GetFilename(originalVersion, filename);
            string newFile = GetFilename(newVersion, filename);
            string deltaFile = Path.GetTempFileName();
            File.WriteAllBytes(deltaFile, deltaData);

            DeltaAPI.ApplyDelta(originalFile, newFile, deltaFile);
        }

        public Uri GetUpdateServerUri()
        {
            string configFilename = Path.Combine(baseDir, "config.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(configFilename);

            return new Uri(doc.SelectSingleNode("config/updateServer").InnerText);
        }

        private string GetVersionDir(string version)
        {
            return Path.Combine(baseDir, version);
        }

        private string GetFilename(string version, string filename)
        {
            return Path.Combine(GetVersionDir(version), filename);
        }

        private static string GetExecutingAssemblyLocation()
        {
            return Assembly.GetExecutingAssembly().Location;
        }

        private string GetConfigValue(string name)
        {
            string configFilename = Path.Combine(baseDir, "config.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(configFilename);

            XmlNode configValue = doc.SelectSingleNode("config/" + name);
            return configValue == null ? string.Empty : configValue.InnerText;
        }

        private void SetConfigValue(string name, string value)
        {
            string configFilename = Path.Combine(baseDir, "config.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(configFilename);
            XmlNode lastVersionNode = doc.SelectSingleNode("config/" + name);
            if (lastVersionNode == null)
            {
                lastVersionNode = doc.CreateElement(name);
                doc.SelectSingleNode("config").AppendChild(lastVersionNode);
            }

            lastVersionNode.InnerText = value;
            doc.Save(configFilename);
        }

        private ResultMergeConfigs MergeNewKeysFromConfig(string newConfigPath)
        {
            string configFilename = Path.Combine(baseDir, "config.xml");

            XmlDocument oldDoc = new XmlDocument();
            oldDoc.Load(configFilename);

            XmlDocument newDoc = new XmlDocument();
            newDoc.Load(newConfigPath);

            XmlNode oldMods = oldDoc.SelectSingleNode("/config/mods");
            XmlNode newMods = newDoc.SelectSingleNode("/config/mods");

            var addedKeys = new ResultMergeConfigs();

            foreach (XmlNode newMod in newMods.ChildNodes)
            {
                string modName = newMod.Name;
                XmlNode oldMod = oldMods.SelectSingleNode(modName);

                if (oldMod != null)
                {
                    foreach (XmlNode newChild in newMod.ChildNodes)
                    {
                        if (oldMod.SelectSingleNode(newChild.Name) == null)
                        {
                            XmlNode imported = oldDoc.ImportNode(newChild, true);
                            oldMod.AppendChild(imported);

                            addedKeys.Result.Add(new MergeConfigKeyValues()
                            {
                                ModName = modName,
                                Key = newChild.Name,
                                Value = newChild.InnerText
                            });
                        }
                    }
                }
                else
                {
                    XmlNode importedMod = oldDoc.ImportNode(newMod, true);
                    oldMods.AppendChild(importedMod);

                    foreach (XmlNode child in newMod.ChildNodes)
                    {
                        addedKeys.Result.Add(new MergeConfigKeyValues()
                        {
                            ModName = modName,
                            Key = child.Name,
                            Value = child.InnerText
                        });
                    }
                }
            }

            oldDoc.Save(configFilename);

            return addedKeys;
        }

    }
}
