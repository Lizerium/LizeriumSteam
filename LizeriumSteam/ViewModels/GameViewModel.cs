/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 02 апреля 2026 07:06:20
 * Version: 1.0.7
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

using HandyControl.Controls;
using HandyControl.Data;

using LizeriumSteam.Events;
using LizeriumSteam.Events.Unpack;
using LizeriumSteam.Models.Games;
using LizeriumSteam.Models.Games.Changers;
using LizeriumSteam.Services.Games.GameProjectsService;
using LizeriumSteam.Services.Settings;

using Microsoft.Extensions.Logging;

using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;

namespace LizeriumSteam.ViewModels
{
    public class GameViewModel : BindableBase
    {
        #region props

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private Visibility _isVisibleLoading = Visibility.Visible;

        public Visibility IsVisibleLoading
        {
            get => _isVisibleLoading;
            set => SetProperty(ref _isVisibleLoading, value);
        }

        private bool _isUpdatesOpen = false;

        public bool IsUpdatesOpen
        {
            get { return _isUpdatesOpen; }
            set => SetProperty(ref _isUpdatesOpen, value);
        }

        private ChangeModel _selectedUpdate;

        public ChangeModel SelectedVersionChangesBlock
        {
            get { return _selectedUpdate; }
            set => SetProperty(ref _selectedUpdate, value);
        }

        private string _titleSelectedProject;

        public string TitleSelectedProject
        {
            get { return _titleSelectedProject; }
            set => SetProperty(ref _titleSelectedProject, value);
        }


        #endregion

        public ObservableCollection<GameButtonViewModel> GameButtons { get; set; } 
            = new ObservableCollection<GameButtonViewModel>();
        public ObservableCollection<GameButtonViewModel> PagedGameButtons { get; set; } = 
            new ObservableCollection<GameButtonViewModel>();
        public ObservableCollection<ChangeModel> ChangesProject { get; set; }
            = new ObservableCollection<ChangeModel>();


        private readonly IAppSettings _appSettings;
        private readonly IEventAggregator _eventAggregator;
        private readonly IProjectsService _gameService;
        private readonly GameButtonViewModelFactory _viewModelFactory;
        private readonly ILogger<GameViewModel> _logger;
        private int _pageIndex;
        public int PageIndex
        {
            get => _pageIndex;
            set
            {
                SetProperty(ref _pageIndex, value);
                UpdatePagedItems();
            }
        }
        public ICommand PageUpdatedCmd => new DelegateCommand(() => UpdatePagedItems());
        public DelegateCommand CloseUpdatesDrawerCommand { get; }

        private const int PageSize = 16;

        private int _pageCount;
        public int PageCount
        {
            get => _pageCount;
            set => SetProperty(ref _pageCount, value);
        }

        private void UpdatePagedItems()
        {
            PagedGameButtons.Clear();
            var index = PageIndex;
            if (PageIndex == 0)
                index = 0;
            else
                index -= 1;

            // Вычисляем сколько всего страниц
            PageCount = (int)Math.Ceiling((double)GameButtons.Count / PageSize);

            foreach (var item in GameButtons.Skip(index * PageSize).Take(PageSize))
            {
                PagedGameButtons.Add(item);
            }
        }

        public GameViewModel(IAppSettings appSettings, 
            IEventAggregator eventAggregator,
            ILogger<GameViewModel> logger,
            GameButtonViewModelFactory viewModelFactory,
            IProjectsService gameService)
        {
            _appSettings = appSettings;
            _eventAggregator = eventAggregator;
            _gameService = gameService;
            _viewModelFactory = viewModelFactory;
            _logger = logger;

            // подписываемся на событие готовности
            _eventAggregator.GetEvent<AppReadyEvent>()
                .Subscribe(OnAppReady, ThreadOption.UIThread);

            // Подписка на событие изменения языка
            _eventAggregator.GetEvent<LanguageChangedEvent>().Subscribe(OnLanguageChanged);

            // Подписка на событие просмотра обновлений проекта
            _eventAggregator.GetEvent<OpenProjectUpdatesInfoEvent>().Subscribe(OpenProjectUpdatesInfo);

            #region Unpack States

            // Уведомление о возможно медленной распаковке первичного архива
            _eventAggregator.GetEvent<WarningLowTimeOperationEvent>()
                .Subscribe(WarningLowTimeOperation, ThreadOption.UIThread);

            // Уведомление о возможно медленной распаковке вторичных архивов
            _eventAggregator.GetEvent<WarningUnpackZipOperationEvent>()
                .Subscribe(WarningUnpackZipOperation, ThreadOption.UIThread);

            // Уведомление о успешно распаковке первичного архива
            _eventAggregator.GetEvent<SuccessUnpackTarOperationEvent>()
                .Subscribe(SuccessUnpackTarOperation, ThreadOption.UIThread);

            // Уведомление о успешно распаковке вторичных архивов
            _eventAggregator.GetEvent<SuccessUnpackZipOperationEvent>()
                .Subscribe(SuccessUnpackZipOperation, ThreadOption.UIThread);

            #endregion

            CloseUpdatesDrawerCommand = new DelegateCommand(CloseUpdatesDrawer);
        }

        private void CloseUpdatesDrawer()
        {
            IsUpdatesOpen = false;
        }

        private void OpenProjectUpdatesInfo(GameButtonModel model)
        {
            try
            {
                if (File.Exists(model.FileInfoUpdatesPath))
                {
                    var ch = File.ReadAllText(model.FileInfoUpdatesPath);
                    var updates = JsonSerializer.Deserialize<ChangesProjectDataModel>(ch);

                    if (updates.Updates.Count > 0)
                    {
                        foreach (var data in updates.Updates)
                        {
                            var d = data.data;
                            foreach (var i in d)
                            {
                                i.Category = i.GetCategory(updates.Categories);
                            }
                        }

                        ChangesProject.Clear();
                        foreach (var update in updates.Updates)
                            ChangesProject.Add(update);

                        TitleSelectedProject = model.TitleView;
                        SelectedVersionChangesBlock = ChangesProject[0];
                        IsUpdatesOpen = true;
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error read updates_info.json");
            }
        }

        private void OnLanguageChanged(string newLang)
        {
            Title = Application.Current.TryFindResource("GameViewTitleText") as string ?? "Игры";
        }

        private async void OnAppReady()
        {
            Title = Application.Current.TryFindResource("GameViewTitleText") as string ?? "Игры";

            await _gameService.Initialize();
            IsVisibleLoading = Visibility.Collapsed;
            await Task.Delay(100);

            foreach (var game in _gameService.GameButtons) // тут у тебя пока модели
            {
                var vm = _viewModelFactory.Create(game);
                GameButtons.Add(vm);
            }
          
            UpdatePagedItems();
        }

        #region Events

        private void SuccessUnpackTarOperation()
        {
            var msgAudioError = (Application.Current.TryFindResource("SuccessUnpackTarOperationEventText") as string ??
                                                 "Распаковка первичного архива обновления прошла успешно!");
            var growlInfoAudio = new GrowlInfo
            {
                Type = InfoType.Error,
                Message = msgAudioError,
                StaysOpen = true,
                WaitTime = 10
            };
            Growl.Success(growlInfoAudio);
        }

        private void SuccessUnpackZipOperation()
        {
            var msgAudioError = (Application.Current.TryFindResource("SuccessUnpack7zOperationEventText") as string ??
                                                  "Распаковка вторичных архивов обновления прошла успешно!");
            var growlInfoAudio = new GrowlInfo
            {
                Type = InfoType.Error,
                Message = msgAudioError,
                StaysOpen = true,
                WaitTime = 10
            };
            Growl.Success(growlInfoAudio);
        }

        private void WarningUnpackZipOperation()
        {
            var msgAudioError = (Application.Current.TryFindResource("WarningUnpack7zOperationEventText") as string ??
                                                 "Пожалуйста подождите, распаковка вторичных архивов может занять некоторое время!");
            var growlInfoAudio = new GrowlInfo
            {
                Type = InfoType.Error,
                Message = msgAudioError,
                StaysOpen = true,
                WaitTime = 10
            };
            Growl.Warning(growlInfoAudio);
        }

        private void WarningLowTimeOperation()
        {
            var msgAudioError = (Application.Current.TryFindResource("WarningLowTimeOperationEventText") as string ??
                                                "Пожалуйста подождите, распаковка первичного архива может занять некоторое время!");
            var growlInfoAudio = new GrowlInfo
            {
                Type = InfoType.Error,
                Message = msgAudioError,
                StaysOpen = true,
                WaitTime = 10
            };
            Growl.Warning(growlInfoAudio);
        }

        #endregion
    }
}
