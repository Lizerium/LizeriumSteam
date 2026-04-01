/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 01 апреля 2026 13:19:48
 * Version: 1.0.5
 */

using System;
using System.Diagnostics;
using System.Windows;

using AppUpdater.Chef;

using LizeriumSteam.Models.Games;
using LizeriumSteam.Models.Games.GameUpdate;

namespace LizeriumSteam.Services.Tray.Windows
{
    /// <summary>
    /// Сервис отображение уведомлений системы в Windows
    /// </summary>
    public class TrayAlertsService : ITrayAlertsService
    {
        private readonly ITrayService _trayService;

        public TrayAlertsService(ITrayService trayService)
        {
            _trayService = trayService;
        }

        private bool IsErrorConnectAlert = false;

        public void ShowTrayCompleteUpdate(StateErrorUpdate state)
        {
            var title = Application.Current.TryFindResource("AppUpdateCompletedAlertTitleText") as string
                ?? "Состояние обновления";
            var content = "";
            var type = System.Windows.Forms.ToolTipIcon.Info;

            switch (state)
            {
                case StateErrorUpdate.Success:
                    content = Application.Current.TryFindResource("AppUpdateCompletedAlertSuccessText") as string
                        ?? "Обновление завершено! Пожалуйста перезапустите приложение...";
                    break;
                case StateErrorUpdate.ErrorSaveFile:
                    content = Application.Current.TryFindResource("AppUpdateCompletedAlertErrorSaveFileText") as string
                        ?? "Обновление прервано! Ошибка сохранения файла...";
                    type = System.Windows.Forms.ToolTipIcon.Error;
                    break;
                case StateErrorUpdate.ErrorUnpackFile:
                    content = Application.Current.TryFindResource("AppUpdateCompletedAlertErrorUnpackFileText") as string
                       ?? "Обновление прервано! Ошибка распаковки файлов...";
                    type = System.Windows.Forms.ToolTipIcon.Error;
                    break;
                case StateErrorUpdate.ErrorConnectServer:
                    content = Application.Current.TryFindResource("AppUpdateCompletedAlertErrorConnectServerText") as string
                      ?? "Обновление невозможно! Ошибка подключения к интернету...";
                    type = System.Windows.Forms.ToolTipIcon.Error;
                    break;
            }

            if (!IsErrorConnectAlert && state == StateErrorUpdate.ErrorConnectServer)
            {
                // 🔔 Отправляем уведомление в трей
                _trayService.ShowBalloonTip(
                    title, // тот же Token, что в XAML
                    content,
                    type,
                    5
                );
                IsErrorConnectAlert = true;
            }

            if (state == StateErrorUpdate.ErrorUnpackFile ||
                state == StateErrorUpdate.ErrorSaveFile ||
                state == StateErrorUpdate.Success)
            {
                // 🔔 Отправляем уведомление в трей
                _trayService.ShowBalloonTip(
                    title, // тот же Token, что в XAML
                    content,
                    type,
                    3
                );
            }

            if (state == StateErrorUpdate.Success)
                IsErrorConnectAlert = false;
        }

        public void ShowTrayUpdateAvailable(string version)
        {
            var title = Application.Current.TryFindResource("AppUpdateAvailableTitleServerText") as string
                ?? "Обновление приложения";
            var content = ((Application.Current.TryFindResource("AppUpdateAvailableContentServerText") as string
            ?? "Выпущена новая версия: ")) + " " + version;

            // 🔔 Отправляем уведомление в трей
            _trayService.ShowBalloonTip(

                title,
                content,
                System.Windows.Forms.ToolTipIcon.Info,
                3
            );
        }

        public void ShowTrayCompleteUpdateProject(ProcessUpdateGameModel process)
        {
            var title = Application.Current.TryFindResource("AppUpdateAvailableTitleServerText") as string
            ?? "Обновление приложения";
            var content = ((Application.Current.TryFindResource("CompleteUpdateProjectAlertSuccessText_1") as string
            ?? "Обновление для проекта")) + " " + process.TitleView + $" [{process.OldVersion}]->[{process.NewVersion}]"
            + " " + ((Application.Current.TryFindResource("CompleteUpdateProjectAlertSuccessText_2") as string
            ?? "успешно завершено!"));

            // 🔔 Отправляем уведомление в трей
            _trayService.ShowBalloonTip(
                title, // тот же Token, что в XAML
                content,
                System.Windows.Forms.ToolTipIcon.Info,
                3
            );
        }

        public void ShowTrayCompleteDownloadProject(ProcessUpdateGameModel process)
        {
            var title = Application.Current.TryFindResource("AppDownloadAvailableTitleServerText") as string
            ?? "Скачивание приложения";
            var content = ((Application.Current.TryFindResource("AppDownloadAvailableTitleServerText") as string
            ?? "Скачивание приложения")) + " " + process.TitleView + $" [{process.OldVersion}]"
            + " " + ((Application.Current.TryFindResource("CompleteUpdateProjectAlertSuccessText_2") as string
            ?? "успешно завершено!"));

            // 🔔 Отправляем уведомление в трей
            _trayService.ShowBalloonTip(
                title, // тот же Token, что в XAML
                content,
                System.Windows.Forms.ToolTipIcon.Info,
                3
            );
        }

        public void ShowTrayCompleteInstallProject(GameButtonModel process)
        {
            var title = Application.Current.TryFindResource("AppDeployAvailableTitleServerText") as string
            ?? "Развёртывание приложения";
            var content = ((Application.Current.TryFindResource("AppDeployAvailableTitleServerText") as string
            ?? "Развёртывание приложения")) + " " + process.TitleView + $" [{process.LastVersionInstall}]"
            + " " + ((Application.Current.TryFindResource("CompleteUpdateProjectAlertSuccessText_2") as string
            ?? "успешно завершено!"));

            // 🔔 Отправляем уведомление в трей
            _trayService.ShowBalloonTip(
                title, // тот же Token, что в XAML
                content,
                System.Windows.Forms.ToolTipIcon.Info,
                3
            );
        }

        public void ShowTrayNotCompleteInstallProject(GameButtonModel process)
        {
            var title = Application.Current.TryFindResource("AppDeployAvailableTitleServerText") as string
            ?? "Развёртывание приложения";
            var content = ((Application.Current.TryFindResource("AppDeployAvailableTitleServerText") as string
            ?? "Развёртывание приложения")) + " " + process.TitleView + $" [{process.LastVersionInstall}]"
            + " " + ((Application.Current.TryFindResource("AppNotDeployAvailableTitleServerText") as string
            ?? "невозможно, так как файлы установки не найдены!"));

            // 🔔 Отправляем уведомление в трей
            _trayService.ShowBalloonTip(
                title, // тот же Token, что в XAML
                content,
                System.Windows.Forms.ToolTipIcon.Error,
                3
            );
        }

        public void ShowTrayCancelUpdateProject(ProcessUpdateGameModel process)
        {
            var title = Application.Current.TryFindResource("AppUpdateAvailableTitleServerText") as string
            ?? "Обновление приложения";
            var content = ((Application.Current.TryFindResource("CompleteUpdateProjectAlertSuccessText_1") as string
            ?? "Обновление для проекта")) + " " + process.TitleView + $" [{process.OldVersion}]->[{process.NewVersion}]"
            + " " + ((Application.Current.TryFindResource("CompleteUpdateProjectAlertSuccessText_2_e") as string
            ?? "отменено!"));

            // 🔔 Отправляем уведомление в трей
            _trayService.ShowBalloonTip(
                title, // тот же Token, что в XAML
                content,
                System.Windows.Forms.ToolTipIcon.Warning,
                3
            );
        }


        public void ShowTrayCancelDownloadProject(ProcessUpdateGameModel process)
        {
            var title = Application.Current.TryFindResource("AppDownloadAvailableTitleServerText") as string
            ?? "Скачивание приложения";
            var content = ((Application.Current.TryFindResource("AppDownloadAvailableTitleServerText") as string
            ?? "Скачивание приложения")) + " " + process.TitleView + $" [{process.OldVersion}]"
            + " " + ((Application.Current.TryFindResource("CompleteUpdateProjectAlertSuccessText_2_e") as string
            ?? "отменено!"));

            // 🔔 Отправляем уведомление в трей
            _trayService.ShowBalloonTip(
                title, // тот же Token, что в XAML
                content,
                System.Windows.Forms.ToolTipIcon.Warning,
                3
            );
        }

        public void ShowTrayNotAvailableUpdateProject(ProcessUpdateGameModel process)
        {
            var title = Application.Current.TryFindResource("CompleteNotAvailableAlertSuccessText_1") as string
            ?? "Обновление * Скачивание приложения";
            var content = ((Application.Current.TryFindResource("CompleteNotAvailableAlertSuccessText_2") as string
            ?? "Нет доступных версий для скачивания у")) + " " + process.TitleView;

            // 🔔 Отправляем уведомление в трей
            _trayService.ShowBalloonTip(
                title, // тот же Token, что в XAML
                content,
                System.Windows.Forms.ToolTipIcon.Warning,
                3
            );
        }
    }
}
