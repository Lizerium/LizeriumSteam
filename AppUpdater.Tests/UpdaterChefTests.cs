/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 13 мая 2026 13:35:52
 * Version: 1.0.50
 */

using AppUpdater.LocalStructure;
using AppUpdater.Recipe;
using AppUpdater.Server;
using AppUpdater.Chef;
using NUnit.Framework;
using Rhino.Mocks;
using AppUpdater.Utils;
using System.Threading.Tasks;
using System.Threading;

namespace AppUpdater.Tests
{
    [TestFixture]
    public class UpdaterChefTests
    {
        private UpdaterChef updaterChef;
        private ILocalStructureManager localStructureManager;
        private IUpdateServer updateServer;

        [SetUp]
        public void Setup()
        {
            localStructureManager = MockRepository.GenerateStub<ILocalStructureManager>();
            updateServer = MockRepository.GenerateStub<IUpdateServer>();
            updaterChef = new UpdaterChef(localStructureManager, updateServer);
        }

        [Test]
        public async void Cook_WithAVersionAlreadyDownloaded_CreatesTheVersionDirectory()
        {
            localStructureManager.Stub(x => x.HasVersionFolder("2.0.0")).Return(true);
            UpdateRecipe updateRecipe = new UpdateRecipe("2.0.0", "1.0.0", new UpdateRecipeFile[0]);
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            await updaterChef.Cook(updateRecipe, cancellationTokenSource.Token);

            localStructureManager.AssertWasCalled(x => x.DeleteVersionDir("2.0.0"));
        }

        [Test]
        public async void Cook_CreatesTheVersionDirectory()
        {
            UpdateRecipe updateRecipe = new UpdateRecipe("2.0.0", "1.0.0", new UpdateRecipeFile[0]);
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            await updaterChef.Cook(updateRecipe, cancellationTokenSource.Token);

            localStructureManager.AssertWasCalled(x => x.CreateVersionDir("2.0.0"));
        }

        [Test]
        public async void Cook_CopyExistingFiles()
        {
            UpdateRecipeFile file1 = new UpdateRecipeFile("test1.txt", "123", 100, FileUpdateAction.Copy, null);
            UpdateRecipeFile file2 = new UpdateRecipeFile("test2.txt", "123", 100, FileUpdateAction.Download, "test2.txt.deploy");
            UpdateRecipe updateRecipe = new UpdateRecipe("2.0.0", "1.0.0", new UpdateRecipeFile[] { file1, file2 });
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            await updaterChef.Cook(updateRecipe, cancellationTokenSource.Token);

            localStructureManager.AssertWasCalled(x => x.CopyFile("1.0.0", "2.0.0", "test1.txt"));
        }

        [Test]
        public async void Cook_SavesNewFiles()
        {
            byte[] data = new byte[]{1,2,3,4,5};
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var st = updateServer.Stub(async x => await x.DownloadFile(cancellationTokenSource.Token, "2.0.0", "test2.txt.deploy"));
            st.Return(Task.FromResult(DataCompressor.Compress(data)));
            UpdateRecipeFile file1 = new UpdateRecipeFile("test1.txt", "123", 100, FileUpdateAction.Copy, null);
            UpdateRecipeFile file2 = new UpdateRecipeFile("test2.txt", "123", 100, FileUpdateAction.Download, "test2.txt.deploy");
            UpdateRecipe updateRecipe = new UpdateRecipe("2.0.0", "1.0.0", new UpdateRecipeFile[] { file1, file2 });

            await updaterChef.Cook(updateRecipe, cancellationTokenSource.Token);

            localStructureManager.AssertWasCalled(x => x.SaveFile("2.0.0", "test2.txt", data));
        }

        [Test]
        public async void Cook_ApplyDeltaInModifiedFiles()
        {
            byte[] data = new byte[] { 1, 2, 3, 4, 5 };
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var st = updateServer.Stub(async x => await x.DownloadFile(cancellationTokenSource.Token, "2.0.0", "test2.txt.deploy"));
            st.Return(Task.FromResult(data));
            UpdateRecipeFile file1 = new UpdateRecipeFile("test1.txt", "123", 100, FileUpdateAction.Copy, null);
            UpdateRecipeFile file2 = new UpdateRecipeFile("test2.txt", "123", 100, FileUpdateAction.DownloadDelta, "test2.txt.deploy");
            UpdateRecipe updateRecipe = new UpdateRecipe("2.0.0", "1.0.0", new UpdateRecipeFile[] { file1, file2 });

            await updaterChef.Cook(updateRecipe, cancellationTokenSource.Token);

            localStructureManager.AssertWasCalled(x => x.ApplyDelta("1.0.0", "2.0.0", "test2.txt", data));
        }
    }
}
