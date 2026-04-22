/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 22 апреля 2026 18:58:37
 * Version: 1.0.29
 */

using LizeriumSteam.Models.Games;
using LizeriumSteam.Models.Games.GameInstall;

using Prism.Events;

namespace LizeriumSteam.Events
{
    public class AppGameIsAvailableToInstallEvent : PubSubEvent<ProcessInstallGameModel> { }
}
