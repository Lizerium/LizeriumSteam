/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 30 марта 2026 12:35:16
 * Version: 1.0.
 */

using LizeriumSteam.Models.Games;

using Prism.Events;

namespace LizeriumSteam.Events
{
    public class OpenProjectUpdatesInfoEvent : PubSubEvent<GameButtonModel> { }
}
