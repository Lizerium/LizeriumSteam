/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 18 сентября 2026 08:19:20
 * Version: 1.0.175
 */

using AppUpdater.Chef;

using Prism.Events;

namespace LizeriumSteam.Events
{
    public class AppUpdateCompletedEvent : PubSubEvent<StateErrorUpdate> { }
}
