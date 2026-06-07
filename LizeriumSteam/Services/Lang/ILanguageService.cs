/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 07 июня 2026 18:42:45
 * Version: 1.0.74
 */

namespace LizeriumSteam.Services.Lang
{
    public interface ILanguageService
    {
        string CurrentLanguage { get; set; }
        void ApplyLanguage(string cultureCode);
    }
}
