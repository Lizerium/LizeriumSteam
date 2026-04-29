/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 29 апреля 2026 07:13:25
 * Version: 1.0.36
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AppUpdater.Utils;

namespace AppUpdater.Tests.Utils
{
    [TestFixture]
    public class PathUtilsTests
    {
        [Test]
        public void AddTrailingSlash_WithAPathThatContainsTheSlash_DoNotAddAnotherSlash()
        {
            string path = @"C:\teste\";

            string processedPath = PathUtils.AddTrailingSlash(path);

            Assert.That(processedPath, Is.EqualTo(@"C:\teste\"));
        }

        [Test]
        public void AddTrailingSlash_WithAPathThatDoesNOTContainTheSlash_AddTheSlash()
        {
            string path = @"C:\teste\";

            string processedPath = PathUtils.AddTrailingSlash(path);

            Assert.That(processedPath, Is.EqualTo(@"C:\teste\"));
        }
    }
}
