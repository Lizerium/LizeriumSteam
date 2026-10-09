/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 09 октября 2026 11:34:41
 * Version: 1.0.196
 */

using LizeriumSteam.Models.Games.GameUpdate;

using Prism.Events;

namespace LizeriumSteam.Events
{
    public class AppGameUnpackFilesEvent : PubSubEvent<ProcessUnpackFilesGamesModel> { }
}
