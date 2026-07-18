/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 18 июля 2026 07:48:45
 * Version: 1.0.114
 */

using LizeriumSteam.Models.Games;
using LizeriumSteam.Models.Games.GameInstall;

using Prism.Events;

namespace LizeriumSteam.Events
{
    public class AppGameIsAvailableToInstallEvent : PubSubEvent<ProcessInstallGameModel> { }
}
