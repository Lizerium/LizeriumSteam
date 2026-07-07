/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 07 июля 2026 12:42:05
 * Version: 1.0.103
 */

using LizeriumSteam.Models.Games.GameUpdate;

using Prism.Events;

namespace LizeriumSteam.Events
{
    public class AppGameUpdateFilesEvent : PubSubEvent<ProcessFilesUpdateGameMode> { }
}
