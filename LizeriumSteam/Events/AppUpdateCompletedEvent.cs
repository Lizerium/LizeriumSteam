/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 23 июня 2026 16:15:35
 * Version: 1.0.90
 */

using AppUpdater.Chef;

using Prism.Events;

namespace LizeriumSteam.Events
{
    public class AppUpdateCompletedEvent : PubSubEvent<StateErrorUpdate> { }
}
