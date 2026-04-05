/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 05 апреля 2026 15:18:02
 * Version: 1.0.10
 */

using LizeriumSteam.Models.Games;
using LizeriumSteam.Models.Games.GameInstall;

using Prism.Events;

namespace LizeriumSteam.Events
{
    public class AppGameIsAvailableToInstallEvent : PubSubEvent<ProcessInstallGameModel> { }
}
