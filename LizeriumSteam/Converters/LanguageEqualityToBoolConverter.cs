/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 16 апреля 2026 11:59:53
 * Version: 1.0.23
 */

using System;
using System.Globalization;
using System.Windows.Data;

namespace LizeriumSteam.Converters
{
    public class LanguageEqualityToBoolConverter : IMultiValueConverter
    {
        // values[0] = SelectedLanguage
        // values[1] = язык пункта меню
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || values[0] == null || values[1] == null) return true;
            return !values[0].Equals(values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
