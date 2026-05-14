/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 14 мая 2026 10:55:54
 * Version: 1.0.51
 */

using System.Windows.Controls;

using LizeriumSteam.ViewModels;

namespace LizeriumSteam.Views
{
    /// <summary>
    /// Логика взаимодействия для GameButtonView.xaml
    /// </summary>
    public partial class GameButtonView : UserControl
    {
        public GameButtonView()
        {
            InitializeComponent();
        }

        private void ButtonConfig_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.ContextMenu != null)
            {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                button.ContextMenu.IsOpen = true;
            }
        }

        private void ContextMenu_Closed(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is GameButtonViewModel vm)
            {
                vm.IsLangMenuOpen = false;
            }
        }

        private void ContextMenu_Opened(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is GameButtonViewModel vm)
            {
                vm.IsLangMenuOpen = true;
            }
        }
    }
}
