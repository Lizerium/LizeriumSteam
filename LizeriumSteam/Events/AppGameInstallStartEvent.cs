/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 01 сентября 2026 08:53:46
 * Version: 1.0.158
 */

using LizeriumSteam.Models.Games;

using Prism.Events;

namespace LizeriumSteam.Events
{
    public class AppGameInstallStartEvent : PubSubEvent<GameButtonModel> { }
}
