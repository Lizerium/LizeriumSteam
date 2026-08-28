/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 28 августа 2026 07:12:59
 * Version: 1.0.155
 */

using LizeriumSteam.Models.Games;
using LizeriumSteam.Models.Games.GameInstall;

using Prism.Events;

namespace LizeriumSteam.Events
{
    public class AppGameIsAvailableToInstallEvent : PubSubEvent<ProcessInstallGameModel> { }
}
