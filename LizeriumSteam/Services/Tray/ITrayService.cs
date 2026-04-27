/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 27 апреля 2026 10:02:12
 * Version: 1.0.34
 */

using System;
using System.Windows.Forms;

namespace LizeriumSteam.Services.Tray
{
    public interface ITrayService : IDisposable
    {
        void ShowBalloonTip(string title, string text, ToolTipIcon icon = ToolTipIcon.Info, int time = 5);
        void Initialize();
        void ShowWindow();
        void ExitApplication();
        void Dispose();
    }
}
