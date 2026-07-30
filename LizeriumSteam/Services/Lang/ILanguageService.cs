/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 30 июля 2026 07:10:26
 * Version: 1.0.126
 */

namespace LizeriumSteam.Services.Lang
{
    public interface ILanguageService
    {
        string CurrentLanguage { get; set; }
        void ApplyLanguage(string cultureCode);
    }
}
