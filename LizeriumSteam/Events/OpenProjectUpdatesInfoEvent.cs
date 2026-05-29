/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 29 мая 2026 14:16:00
 * Version: 1.0.65
 */

using LizeriumSteam.Models.Games;

using Prism.Events;

namespace LizeriumSteam.Events
{
    public class OpenProjectUpdatesInfoEvent : PubSubEvent<GameButtonModel> { }
}
