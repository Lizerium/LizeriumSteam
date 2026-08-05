/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 05 августа 2026 07:13:58
 * Version: 1.0.132
 */

namespace LizeriumSteam.Services.Lang
{
    public interface ILanguageService
    {
        string CurrentLanguage { get; set; }
        void ApplyLanguage(string cultureCode);
    }
}
