/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 04 мая 2026 07:13:51
 * Version: 1.0.41
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
