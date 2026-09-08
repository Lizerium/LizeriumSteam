/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 08 сентября 2026 06:57:34
 * Version: 1.0.165
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
