/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 18 июля 2026 07:48:45
 * Version: 1.0.114
 */

namespace LizeriumSteam.Services.Lang
{
    public interface ILanguageService
    {
        string CurrentLanguage { get; set; }
        void ApplyLanguage(string cultureCode);
    }
}
