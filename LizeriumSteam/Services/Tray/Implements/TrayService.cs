/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 02 мая 2026 19:36:41
 * Version: 1.0.39
 */

using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Forms;

using LizeriumSteam.Services.Tray.Helps.UI;
using LizeriumSteam.Views;

using ApplicationWindow = System.Windows.Application;
using FontStyleDrawing = System.Drawing.FontStyle;
using NotifyIconWin = System.Windows.Forms.NotifyIcon;

namespace LizeriumSteam.Services.Tray.Implements
{
    /// <summary>
    /// Сервис отображения в windows трее
    /// </summary>
    public sealed class TrayService : ITrayService
    {
        private NotifyIconWin _trayIcon;

        public void Initialize()
        {
            Dispose();

            var asmName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            var uri = new Uri($"pack://application:,,,/{asmName};component/Resources/logo.ico");

            var stream = ApplicationWindow.GetResourceStream(uri)?.Stream;
            _trayIcon = new NotifyIconWin
            {
                Icon = stream != null ? new Icon(stream) : SystemIcons.Application,
                Text = "Lizerium Steam",
                Visible = true
            };

            var contextMenu = new ContextMenuStrip
            {
                Renderer = new CustomToolStripRenderer(), // твой кастомный рендерер
                Font = new Font("Segoe UI", 17F, FontStyleDrawing.Bold)
            };

            // Показать
            var showItem = new ToolStripMenuItem("Показать");
            using (Stream streamShow = ApplicationWindow.GetResourceStream(
                new Uri("pack://application:,,,/LizeriumLauncher;component/Resources/Buttons/open.png")).Stream)
            {
                showItem.Image = new Bitmap(streamShow);
            }
            showItem.Click += (s, e) => ShowWindow();

            // Выход
            var exitItem = new ToolStripMenuItem("Выход");
            using (Stream streamExit = ApplicationWindow.GetResourceStream(
                new Uri("pack://application:,,,/LizeriumLauncher;component/Resources/Buttons/exit.png")).Stream)
            {
                exitItem.Image = new Bitmap(streamExit);
            }
            exitItem.Font = new Font("Segoe UI", 17F, FontStyleDrawing.Bold);
            exitItem.Click += (s, e) => ExitApplication();

            contextMenu.Items.Add(showItem);
            contextMenu.Items.Add(exitItem);

            _trayIcon.ContextMenuStrip = contextMenu;

            _trayIcon.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                    ShowWindow();
            };
        }

        public void ShowWindow()
        {
            if (ApplicationWindow.Current.MainWindow is MainView mainWindow)
            {
                mainWindow.Show();
                mainWindow.WindowState = WindowState.Normal;
                mainWindow.Activate();
            }
        }

        public void ShowBalloonTip(string title, string text, ToolTipIcon icon = ToolTipIcon.Info, int time = 5)
        {
            if (_trayIcon != null)
            {
                _trayIcon.BalloonTipTitle = title;
                _trayIcon.BalloonTipText = text;
                _trayIcon.BalloonTipIcon = icon;
                var timeWait = 1000 * time;
                _trayIcon.ShowBalloonTip(timeWait);
            }
        }
        public void ExitApplication()
        {
            _trayIcon.Visible = false;
            ApplicationWindow.Current.Shutdown();
        }

        public void Dispose()
        {
            if (_trayIcon != null)
            {
                _trayIcon.Visible = false;
                _trayIcon.Dispose();
                _trayIcon = null;
            }
        }
    }
}
