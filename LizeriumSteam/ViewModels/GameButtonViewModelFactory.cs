/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 12 мая 2026 12:23:07
 * Version: 1.0.49
 */

using LizeriumSteam.Models.Games;
using LizeriumSteam.Services.Games.GameProjectsService;
using LizeriumSteam.Services.Games.GameSaveManagerService;
using LizeriumSteam.Services.Games.GameUpdateService;
using LizeriumSteam.Services.Settings;
using LizeriumSteam.Services.Tray.Windows;

using Microsoft.Extensions.Logging;

using Prism.Events;
using Prism.Ioc;

namespace LizeriumSteam.ViewModels
{
    public class GameButtonViewModelFactory
    {
        private readonly IContainerProvider _container;

        public GameButtonViewModelFactory(IContainerProvider container)
        {
            _container = container;
        }

        public GameButtonViewModel Create(GameButtonModel model)
        {
            var logger = _container.Resolve<ILogger<GameButtonViewModel>>();
            var projectsService = _container.Resolve<IProjectsService>();
            var appSettings = _container.Resolve<IAppSettings>();
            var trayAlertsService = _container.Resolve<ITrayAlertsService>();
            var projectUpdateService = _container.Resolve<IProjectUpdateService>();
            var eventAggregator = _container.Resolve<IEventAggregator>();
            var gameSaveManagerService = _container.Resolve<IGameSaveManagerService>();
            return new GameButtonViewModel(model, logger, projectsService, 
                gameSaveManagerService, appSettings, eventAggregator, trayAlertsService,
                projectUpdateService);
        }
    }
}
