/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 28 мая 2026 17:51:07
 * Version: 1.0.64
 */

using System;
using System.Globalization;
using System.Windows.Data;

using LizeriumSteam.Models.Games;

namespace LizeriumSteam.Converters
{
    public class LanguageToDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LangugesEnum lang)
            {
                switch(lang)
                {
                    case LangugesEnum.ru:
                        return "Русский";
                    case LangugesEnum.en:
                        return "English";
                    default:
                        return lang.ToString();
                }
            }
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
