/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 03 мая 2026 07:12:30
 * Version: 1.0.40
 */

using LizeriumSteam.Models.Games;

using Prism.Events;

namespace LizeriumSteam.Events
{
    public class AppGameInstallStartEvent : PubSubEvent<GameButtonModel> { }
}
