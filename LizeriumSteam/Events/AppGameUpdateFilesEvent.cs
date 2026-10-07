/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 07 октября 2026 06:57:36
 * Version: 1.0.194
 */

using LizeriumSteam.Models.Games.GameUpdate;

using Prism.Events;

namespace LizeriumSteam.Events
{
    public class AppGameUpdateFilesEvent : PubSubEvent<ProcessFilesUpdateGameMode> { }
}
