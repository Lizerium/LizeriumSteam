/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 25 сентября 2026 09:41:03
 * Version: 1.0.182
 */

using LizeriumSteam.Models.Games.GameUpdate;

using Prism.Events;

namespace LizeriumSteam.Events
{
    public class AppGameUpdateFilesEvent : PubSubEvent<ProcessFilesUpdateGameMode> { }
}
