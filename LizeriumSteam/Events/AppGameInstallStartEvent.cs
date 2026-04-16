/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 16 апреля 2026 11:59:53
 * Version: 1.0.23
 */

using LizeriumSteam.Models.Games;

using Prism.Events;

namespace LizeriumSteam.Events
{
    public class AppGameInstallStartEvent : PubSubEvent<GameButtonModel> { }
}
