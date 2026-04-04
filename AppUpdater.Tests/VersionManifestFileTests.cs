/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 04 апреля 2026 13:13:11
 * Version: 1.0.9
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AppUpdater.Manifest;

namespace AppUpdater.Tests
{
    [TestFixture]
    public class VersionManifestFileTests
    {
        private VersionManifestFile file;

        [SetUp]
        public void SetUp()
        {
            file = new VersionManifestFile("", "", 1);
            file.Deltas.Add(new VersionManifestDeltaFile("aa.bb", "AAA", 1));
            file.Deltas.Add(new VersionManifestDeltaFile("bb.bb", "BBB", 1));
        }

        [Test]
        public void GetDeltaFrom_WithValidChecksum_ReturnsTheItem()
        {
            VersionManifestDeltaFile delta = file.GetDeltaFrom("AAA");

            Assert.That(delta, Is.Not.Null);
            Assert.That(delta.Filename, Is.EqualTo("aa.bb"));
        }

        [Test]
        public void GetDeltaFrom_WithInvalidChecksum_ReturnsNull()
        {
            VersionManifestDeltaFile delta = file.GetDeltaFrom("MMMMM");

            Assert.That(delta, Is.Null);
        }
    }
}
