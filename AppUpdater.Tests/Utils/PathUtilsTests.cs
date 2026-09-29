/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 29 сентября 2026 09:46:29
 * Version: 1.0.186
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
