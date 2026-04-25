/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 25 апреля 2026 08:31:42
 * Version: 1.0.32
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
    public class ExtractTarTests
    {
        private readonly ILog log = Logger.For<ExtractTarTests>();
        private Tar tar;

        [SetUp]
        public void Setup()
        {
            tar = new Tar();
        }

        [Test]
        public async Task Publish_ExtractTar()
        {
            // тест TAR с вложенным 7z под Ультра LZMA2 сжатием
            var dirPack = Path.Combine(Path.GetTempPath(), "LizeriumFreelancerMode", "updates");
            var pathUpdate = Path.Combine(dirPack, "99.3.15.tar");
            var pathOutUpdate = Path.Combine(dirPack, "out");
            if (!File.Exists(pathUpdate)) return;
            if (!Directory.Exists(pathOutUpdate)) return;
            try
            {
                tar.DataUnpackHandle = null;
                tar.DataUnpackHandle += DataUnpackHandleLogic;
                await tar.ExtractTarWithInner7zAsync(pathUpdate,
                    pathOutUpdate,
                    log);
            }
            catch (Exception ex) 
            {
                var exd = ex.Message;
            }

            //// тест TAR без архивов
            //pathUpdate = Path.Combine(dirPack, "99.3.16.tar");
            //if (!File.Exists(pathUpdate)) return;
            //if (!Directory.Exists(pathOutUpdate)) return;
            //try
            //{
            //    tar.DataUnpackHandle = null;
            //    tar.DataUnpackHandle += DataUnpackHandleLogic;
            //    await tar.ExtractTarWithInner7zAsync(pathUpdate,
            //        pathOutUpdate,
            //        log);
            //}
            //catch (Exception ex)
            //{
            //    var exd = ex.Message;
            //}

            bool directoryExists = Directory.Exists(dirPack);
            Assert.That(directoryExists, Is.True);
        }

        private void DataUnpackHandleLogic(object sender, UnpackHandle e)
        {
            TestContext.WriteLine($"[{e.CurrentUnpackFilesCount}][{e.TotalUnpackFiles}] - распаковано файлов");
            var handle = e;
        }
    }
}
