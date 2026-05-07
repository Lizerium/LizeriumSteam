/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 07 мая 2026 15:46:52
 * Version: 1.0.44
 */

using AppUpdater.LocalStructure;
using AppUpdater.Manifest;
using AppUpdater.Recipe;
using AppUpdater.Server;
using AppUpdater.Chef;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace AppUpdater.Tests
{
    public class UpdateManagerTests
    {
        [TestFixture]
        public class NotInitialized
        {
            private UpdateManager updateManager;
            private IUpdateServer updateServer;
            private ILocalStructureManager localStructureManager;

            [SetUp]
            public void Setup()
            {
                updateServer = MockRepository.GenerateStub<IUpdateServer>();
                localStructureManager = MockRepository.GenerateStub<ILocalStructureManager>();
                var updaterChef = MockRepository.GenerateStub<IUpdaterChef>();
                updateManager = new UpdateManager(updateServer, localStructureManager, updaterChef);
            }

            [Test]
            public void Initialize_LoadsTheCurrentVersion()
            {
                localStructureManager.Stub(x => x.GetCurrentVersion()).Return("1.3.4");

                updateManager.Initialize();

                Assert.That(updateManager.CurrentVersion, Is.EqualTo("1.3.4"));
            }
        }

        [TestFixture]
        public class Initialized
        {
            private UpdateManager updateManager;
            private IUpdateServer updateServer;
            private ILocalStructureManager localStructureManager;
            private IUpdaterChef updaterChef;
            private string initialVersion;
            private string[] installedVersions;

            [SetUp]
            public void Setup()
            {
                updateServer = MockRepository.GenerateStub<IUpdateServer>();
                localStructureManager = MockRepository.GenerateStub<ILocalStructureManager>();
                updaterChef = MockRepository.GenerateStub<IUpdaterChef>();
                updateManager = new UpdateManager(updateServer, localStructureManager, updaterChef);

                initialVersion = "1.2.3";
                installedVersions = new string[] { "1.0.0", "1.1.1", "1.2.3" };
                localStructureManager.Stub(x => x.GetCurrentVersion()).Return(initialVersion);
                localStructureManager.Stub(x => x.GetExecutingVersion()).Return(initialVersion);
                localStructureManager.Stub(x => x.GetInstalledVersions()).Do(new Func<string[]>(()=>installedVersions));
                updateManager.Initialize();
            }


            [Test]
            public async void CheckForUpdate_WithoutUpdate_HasUpdateIsFalse()
            {
                updateServer.Stub(x => x.GetCurrentVersion()).Return(Task.FromResult(initialVersion));

                UpdateInfo updateInfo = await updateManager.CheckForUpdate();

                Assert.That(updateInfo.HasUpdate, Is.False);
            }

            [Test]
            public async void CheckForUpdate_WithoutUpdate_ReturnsTheCurrentVersion()
            {
                updateServer.Stub(x => x.GetCurrentVersion()).Return(Task.FromResult(initialVersion));

                UpdateInfo updateInfo = await updateManager.CheckForUpdate();

                Assert.That(updateInfo.Version, Is.EqualTo(initialVersion));
            }

            [Test]
            public async void CheckForUpdate_WithUpdate_HasUpdateIsTrue()
            {
                updateServer.Stub(x => x.GetCurrentVersion()).Return(Task.FromResult("2.6.8"));

                UpdateInfo updateInfo = await updateManager.CheckForUpdate();

                Assert.That(updateInfo.HasUpdate, Is.True);
            }

            [Test]
            public async void CheckForUpdate_WithUpdate_ReturnsTheNewVersionNumber()
            {
                string newVersion = "2.6.8";
                updateServer.Stub(x => x.GetCurrentVersion()).Return(Task.FromResult(newVersion));

                UpdateInfo updateInfo = await updateManager.CheckForUpdate();

                Assert.That(updateInfo.Version, Is.EqualTo(newVersion));
            }

            [Test]
            public async void DoUpdate_ChangesTheCurrentVersion()
            {
                string newVersion = "2.6.8";
                UpdateInfo updateInfo = new UpdateInfo(true, newVersion);
                updateServer.Stub(x => x.GetManifest(newVersion)).Return(Task.FromResult(new VersionManifest(newVersion, new VersionManifestFile[0])));
                localStructureManager.Stub(x => x.LoadManifest(initialVersion)).Return(new VersionManifest(initialVersion, new VersionManifestFile[0]));
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                await updateManager.DoUpdate(updateInfo, cancellationTokenSource.Token);

                Assert.That(updateManager.CurrentVersion, Is.EqualTo(newVersion));
            }

            [Test]
            public async void DoUpdate_SavesTheCurrentVersion()
            {
                string newVersion = "2.6.8";
                UpdateInfo updateInfo = new UpdateInfo(true, newVersion);
                updateServer.Stub(x => x.GetManifest(newVersion)).Return(Task.FromResult(new VersionManifest(newVersion, new VersionManifestFile[0])));
                localStructureManager.Stub(x => x.LoadManifest(initialVersion)).Return(new VersionManifest(initialVersion, new VersionManifestFile[0]));
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                await updateManager.DoUpdate(updateInfo, cancellationTokenSource.Token);

                localStructureManager.AssertWasCalled(x => x.SetCurrentVersion(newVersion));
            }

            [Test]
            public async void DoUpdate_SavesTheLastValidVersionAsTheExecutingBeingExecuted()
            {
                string versionBeingExecuted = initialVersion;
                string newVersion = "2.6.8";
                UpdateInfo updateInfo = new UpdateInfo(true, newVersion);
                updateServer.Stub(x => x.GetManifest(newVersion)).Return(Task.FromResult(new VersionManifest(newVersion, new VersionManifestFile[0])));
                localStructureManager.Stub(x => x.GetCurrentVersion()).Return("2.0.0");
                localStructureManager.Stub(x => x.LoadManifest(initialVersion)).Return(new VersionManifest(initialVersion, new VersionManifestFile[0]));
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                await updateManager.DoUpdate(updateInfo, cancellationTokenSource.Token);

                localStructureManager.AssertWasCalled(x => x.SetLastValidVersion(versionBeingExecuted));
            }

            [Test]
            public async void DoUpdate_ExecutesTheUpdate()
            {
                string newVersion = "2.6.8";
                UpdateInfo updateInfo = new UpdateInfo(true, newVersion);
                updateServer.Stub(x => x.GetManifest(newVersion)).Return(Task.FromResult(new VersionManifest(newVersion, new VersionManifestFile[0])));
                localStructureManager.Stub(x => x.LoadManifest(initialVersion)).Return(new VersionManifest(initialVersion, new VersionManifestFile[0]));
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                await updateManager.DoUpdate(updateInfo, cancellationTokenSource.Token);

                updaterChef.AssertWasCalled(x => x.Cook(Arg<UpdateRecipe>.Is.Anything, cancellationTokenSource.Token));
            }

            [Test]
            public async void DoUpdate_RemovesOldVersions()
            {
                var updateInfo = SetupUpdateToVersion("3.1");
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                await updateManager.DoUpdate(updateInfo, cancellationTokenSource.Token);

                localStructureManager.AssertWasCalled(x => x.DeleteVersionDir("1.0.0"));
                localStructureManager.AssertWasCalled(x => x.DeleteVersionDir("1.1.1"));
            }

            [Test]
            public async void DoUpdate_DoesNotRemoveTheExecutingVersion()
            {
                var updateInfo = SetupUpdateToVersion("3.1");
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                await updateManager.DoUpdate(updateInfo, cancellationTokenSource.Token);

                localStructureManager.AssertWasNotCalled(x => x.DeleteVersionDir(initialVersion));
            }

            [Test]
            public async void DoUpdate_DoesNotRemoveTheNewVersion()
            {
                installedVersions = new string[] { "1.0.0", "1.1.1", "1.2.3", "3.1"};
                var updateInfo = SetupUpdateToVersion("3.1");
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                await updateManager.DoUpdate(updateInfo, cancellationTokenSource.Token);

                localStructureManager.AssertWasNotCalled(x => x.DeleteVersionDir("3.1"));
            }

            [Test]
            public async void DoUpdate_WithAnErrorWhileDeletingTheOldVersion_IgnoresTheError()
            {
                localStructureManager.Stub(x => x.DeleteVersionDir("1.0.0")).Throw(new Exception("Error deliting version."));
                var updateInfo = SetupUpdateToVersion("3.1");
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                await updateManager.DoUpdate(updateInfo, cancellationTokenSource.Token);
            }

            private UpdateInfo SetupUpdateToVersion(string newVersion)
            {
                UpdateInfo updateInfo = new UpdateInfo(true, newVersion);
                updateServer.Stub(x => x.GetManifest(newVersion)).Return(Task.FromResult(new VersionManifest(newVersion, new VersionManifestFile[0])));
                localStructureManager.Stub(x => x.LoadManifest(initialVersion)).Return(new VersionManifest(initialVersion, new VersionManifestFile[0]));
                return updateInfo;
            }
        }
    }
}
