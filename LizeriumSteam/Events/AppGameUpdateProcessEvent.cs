/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 26 июля 2026 15:33:11
 * Version: 1.0.122
 */

using LizeriumSteam.Models.Games.GameUpdate;

using Prism.Events;

namespace LizeriumSteam.Events
{
    public class AppGameUpdateProcessEvent : PubSubEvent<ProcessUpdateGameModel> { }
}
