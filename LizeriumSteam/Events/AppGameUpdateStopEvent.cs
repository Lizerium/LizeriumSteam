/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 27 июля 2026 15:17:30
 * Version: 1.0.123
 */

using LizeriumSteam.Models.Games.GameUpdate;

using Prism.Events;

namespace LizeriumSteam.Events
{
    public class AppGameUpdateStopEvent : PubSubEvent<ProcessUpdateGameModel> { }
}
