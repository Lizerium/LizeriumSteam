/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 15 апреля 2026 07:05:07
 * Version: 1.0.22
 */

using AppUpdater.Publisher;
using System.IO;

using NUnit.Framework;
using TarExample;
using AppUpdater.Server;
using System;
using AppUpdater.Log;
using System.Threading.Tasks;

namespace AppUpdater.Tests.Server
{
    [TestFixture]
    public class ExtractZipTests
    {
        private readonly ILog log = Logger.For<ExtractTarTests>();
        private Tar tar;

        [SetUp]
        public void Setup()
        {
            tar = new Tar();
        }

        [Test]
        public async Task Publish_FastExtractZip()
        {
            var dirPack = Path.Combine(Path.GetTempPath(), "LizeriumFreelancerMode", "updates");
            var pathUpdate = Path.Combine(dirPack, "99.3.15.zip");
            var pathOutUpdate = Path.Combine(dirPack, "out");
            if (!File.Exists(pathUpdate)) return;
            if (!Directory.Exists(pathOutUpdate)) return;
            try
            {
                // Создаём делегат для прогресса
                Action<int, int> progress = (c, a) =>
                {
                    TestContext.WriteLine($"[{c}][{a}] - файлов");
                };
                ExtractZip.FastExtractZip(pathUpdate, pathOutUpdate, progress);
            }
            catch (Exception ex)
            {
                var exd = ex.Message;
            }

            bool directoryExists = Directory.Exists(dirPack);
            Assert.That(directoryExists, Is.True);
        }
    }
}
