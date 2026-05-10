/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 10 мая 2026 08:03:59
 * Version: 1.0.47
 */

using LizeriumSteam.Models.Games.GameUpdate;

using Prism.Events;

namespace LizeriumSteam.Events
{
    public class AppGameUpdateStopEvent : PubSubEvent<ProcessUpdateGameModel> { }
}
