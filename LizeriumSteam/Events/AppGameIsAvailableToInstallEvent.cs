/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 10 апреля 2026 12:48:19
 * Version: 1.0.15
 */

using LizeriumSteam.Models.Games;
using LizeriumSteam.Models.Games.GameInstall;

using Prism.Events;

namespace LizeriumSteam.Events
{
    public class AppGameIsAvailableToInstallEvent : PubSubEvent<ProcessInstallGameModel> { }
}
