/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 06 мая 2026 10:50:54
 * Version: 1.0.43
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;

namespace LizeriumSteam.Starter
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            string dir = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            XmlDocument doc = new XmlDocument();
            doc.Load(Path.Combine(dir, "config.xml"));

            string version = GetConfigValue(doc, "version");
            string lastVersion = GetConfigValue(doc, "last_version");
            string executable = GetConfigValue(doc, "executable");

            bool runLast = args.Any(x => x.Equals("-last", StringComparison.CurrentCultureIgnoreCase));
            if (runLast && lastVersion == null)
            {
                Console.WriteLine("Last version is not defined.");
                runLast = false;
            }

            if (runLast)
            {
                ExecuteApplication(dir, lastVersion, executable, args);
            }
            else
            {
                ExecuteApplication(dir, version, executable, args);
            }
        }

        private static string GetConfigValue(XmlDocument doc, string name)
        {
            var node = doc.SelectSingleNode("config/" + name);
            return node == null ? null : node.InnerText;
        }

        private static void ExecuteApplication(string baseDir, string version, string executable, string[] args)
        {
            string path = Path.Combine(baseDir, version, executable);
            Process.Start(path, String.Join(" ", args));
        }
    }
}
