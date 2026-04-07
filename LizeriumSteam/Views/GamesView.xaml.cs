/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 07 апреля 2026 11:13:20
 * Version: 1.0.12
 */

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using LizeriumSteam.Models.Games.Changers;
using LizeriumSteam.ViewModels;

namespace LizeriumSteam.Views
{
    public partial class GamesView : UserControl
    {
        public GamesView(GameViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void VersionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedUpdate = VersionComboBox.SelectedItem as ChangeModel;
            if (selectedUpdate != null)
            {
                if (DataContext is GameViewModel vm)
                {
                    vm.SelectedVersionChangesBlock = selectedUpdate;
                }
            }
        }
    }
}
