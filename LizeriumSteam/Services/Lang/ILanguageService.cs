/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 21 сентября 2026 09:40:21
 * Version: 1.0.178
 */

namespace LizeriumSteam.Services.Lang
{
    public interface ILanguageService
    {
        string CurrentLanguage { get; set; }
        void ApplyLanguage(string cultureCode);
    }
}
