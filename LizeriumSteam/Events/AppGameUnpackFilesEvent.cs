/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 19 октября 2026 09:58:34
 * Version: 1.0.205
 */

using LizeriumSteam.Models.Games.GameUpdate;

using Prism.Events;

namespace LizeriumSteam.Events
{
    public class AppGameUnpackFilesEvent : PubSubEvent<ProcessUnpackFilesGamesModel> { }
}
