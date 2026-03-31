/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 31 марта 2026 11:07:39
 * Version: 1.0.4
 */

using Prism.Events;

namespace LizeriumSteam.Events
{
    public class LanguageChangedEvent : PubSubEvent<string> { }
}
