/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 16 мая 2026 11:03:42
 * Version: 1.0.52
 */

using System;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace LizeriumSteam.ViewModels
{
    public class LanguageSelectionDialogViewModel : BindableBase, IDialogAware
    {
        public event Action<IDialogResult> RequestClose;

        public DelegateCommand SelectRussianCommand { get; }
        public DelegateCommand SelectEnglishCommand { get; }

        public LanguageSelectionDialogViewModel()
        {
            SelectRussianCommand = new DelegateCommand(() => CloseDialog("ru"));
            SelectEnglishCommand = new DelegateCommand(() => CloseDialog("en"));
        }

        private void CloseDialog(string language)
        {
            var parameters = new DialogParameters();
            parameters.Add("SelectedLanguage", language);
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, parameters));
        }

        public string Title => "Выбор языка";

        public bool CanCloseDialog() => true;
        public void OnDialogClosed() { }
        public void OnDialogOpened(IDialogParameters parameters) { }
    }
}
