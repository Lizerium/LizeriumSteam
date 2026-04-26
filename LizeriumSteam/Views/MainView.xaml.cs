/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 26 апреля 2026 10:11:00
 * Version: 1.0.33
 */
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using HandyControl.Controls;

using LizeriumSteam.Events;
using LizeriumSteam.Services.Settings;
using LizeriumSteam.ViewModels;

using Prism.Events;
using Prism.Services.Dialogs;

using HandyTabItem = HandyControl.Controls.TabItem;

namespace LizeriumSteam.Views
{
    public partial class MainView
    {
        private readonly IEventAggregator _eventAggregator;
        public MainView(GamesView gamesView, 
            IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            InitializeComponent();

            AddTab(gamesView);       

            MainTabControl.SelectedItem = gamesView; // выделяем игры
            PopupConfig.DataContext = this.DataContext;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                e.Cancel = true; // всегда отменяем по умолчанию
                vm.DialogService.ShowDialog("CloseView",
                    r =>
                {
                    if (r.Result == ButtonResult.OK)
                    {
                        // пользователь выбрал "Закрыть"
                        e.Cancel = false;
                        Application.Current.Shutdown();
                    }
                    if (r.Result == ButtonResult.Ignore)
                    {
                        // пользователь выбрал "Свернуть в трей"
                        this.Hide();
                    }
                });
            }
        }

        private void ButtonConfig_Click(object sender, RoutedEventArgs e)
        {
            PopupConfig.IsOpen = !PopupConfig.IsOpen;
        }

        private void AddTab(FrameworkElement control)
        {
            var tabItem = new HandyTabItem
            {
                Content = control,
                DataContext = control.DataContext,
                Padding = new Thickness(20, 0, 0, 0),
            };

            var binding = new Binding("Title")
            {
                Mode = BindingMode.OneWay
            };
            tabItem.SetBinding(HeaderedContentControl.HeaderProperty, binding);

            // задаём иконку программно
            tabItem.SetValue(IconElement.HeightProperty, 16.0);
            tabItem.SetValue(IconElement.WidthProperty, 16.0);
            tabItem.SetValue(IconElement.GeometryProperty, 
                    Application.Current.FindResource("WindowsGeometry") as System.Windows.Media.Geometry);

            MainTabControl.Items.Add(tabItem);
        }

        private void SelectLang_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string newLang)
                _eventAggregator.GetEvent<LanguageChangedEvent>().Publish(newLang);
            PopupConfig.IsOpen = false;
        }
    }
}
