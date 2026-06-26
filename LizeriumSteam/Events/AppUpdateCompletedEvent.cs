/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 26 июня 2026 07:11:52
 * Version: 1.0.93
 */

using AppUpdater.Chef;

using Prism.Events;

namespace LizeriumSteam.Events
{
    public class AppUpdateCompletedEvent : PubSubEvent<StateErrorUpdate> { }
}
