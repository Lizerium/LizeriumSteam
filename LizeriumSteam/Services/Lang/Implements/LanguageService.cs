/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 02 апреля 2026 07:06:20
 * Version: 1.0.7
 */

using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;

namespace LizeriumSteam.Services.Lang.Implements
{
    /// <summary>
    /// Сервис выбора языковых параметров
    /// </summary>
    public class LanguageService : ILanguageService
    {
        public string CurrentLanguage { get; set; } = Properties.Settings.Default.AppLanguage;

        public void ApplyLanguage(string cultureCode)
        {
            if (string.IsNullOrEmpty(cultureCode))
                return;

            CurrentLanguage = cultureCode;

            // Сохраняем выбор пользователя
            Properties.Settings.Default.AppLanguage = cultureCode;
            Properties.Settings.Default.Save();

            // Меняем культуру приложения
            var culture = new CultureInfo(cultureCode);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            // Обновляем все ресурсы
            UpdateResourceDictionary(cultureCode);
        }

        private void UpdateResourceDictionary(string cultureCode)
        {
            string dictPath = $"Resources/StringResources.{cultureCode}.xaml";
            var dict = new ResourceDictionary { Source = new Uri(dictPath, UriKind.Relative) };

            // Удаляем старую словарю
            var oldDict = Application.Current.Resources.MergedDictionaries
                             .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("StringResources."));
            if (oldDict != null)
                Application.Current.Resources.MergedDictionaries.Remove(oldDict);

            Application.Current.Resources.MergedDictionaries.Add(dict);
        }
    }
}
