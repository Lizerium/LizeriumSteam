/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 01 мая 2026 07:14:08
 * Version: 1.0.38
 */

using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

using System;
using System.Windows;

namespace LizeriumSteam.ViewModels
{
    public class CloseDialogWindowModel : BindableBase, IDialogAware
    {
        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public bool ShouldExit { get; private set; } = false;

        public DelegateCommand MinimizeCommand { get; }
        public DelegateCommand ExitCommand { get; }
        public DelegateCommand CloseCommand { get; }

        public CloseDialogWindowModel()
        {
            Title = Application.Current.TryFindResource("CloseViewTitleText") as string ?? "Закрытие";

            MinimizeCommand = new DelegateCommand(OnMinimize);
            ExitCommand = new DelegateCommand(OnExit);
            CloseCommand = new DelegateCommand(OnClose);
        }

        // Главное событие для закрытия
        public event Action<IDialogResult> RequestClose;

        private void OnMinimize()
        {
            ShouldExit = false;
            RequestClose?.Invoke(new DialogResult(ButtonResult.Ignore));
        }

        private void OnExit()
        {
            ShouldExit = true;
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }

        private void OnClose()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
            // Здесь можно очистить ресурсы
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            // Здесь можно принять параметры (например, текст сообщения)
        }
    }
}
