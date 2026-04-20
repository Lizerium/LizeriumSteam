/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 20 апреля 2026 16:38:08
 * Version: 1.0.27
 */

using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Media;

using HandyControl.Data;
using HandyControl.Themes;

using LizeriumSteam.Views;

using HandyMessageBox = HandyControl.Controls.MessageBox;

namespace LizeriumSteam
{
    public partial class App : Application
    {
        private Mutex mutex;
        private Bootstrapper boot;

        protected override void OnStartup(StartupEventArgs e)
        {
            string mutexName = "Lizerium Steam";
            mutex = new Mutex(false, mutexName);

            if (!mutex.WaitOne(0, false))
            {
                var info = new MessageBoxInfo()
                {
                    Caption = "Ошибка",
                    Message = $"Загрузчик Lizerium уже запущен! Посмотрите в свёрнутых приложениях!",
                    ConfirmContent = "Ок",
                    Button = MessageBoxButton.OK
                };
                HandyMessageBox.Show(info);
                Application.Current.Shutdown();
                return;
            }

            ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
            HandyControl.Properties.Langs.Lang.Culture = new CultureInfo("ru");

            base.OnStartup(e);
            boot = new Bootstrapper();
            boot.Run();
        }

        internal void UpdateTheme(ApplicationTheme theme)
        {
            if (ThemeManager.Current.ApplicationTheme != theme)
            {
                ThemeManager.Current.ApplicationTheme = theme;
            }
        }

        internal void UpdateAccent(Brush accent)
        {
            if (ThemeManager.Current.AccentColor != accent)
            {
                ThemeManager.Current.AccentColor = accent;
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            if (mutex != null)
                mutex?.ReleaseMutex(); // Освобождаем мьютекс при выходе из приложения

            boot?.StopServices();
        }
    }
}
