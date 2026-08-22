/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 22 августа 2026 15:02:37
 * Version: 1.0.149
 */

using AppUpdater.Chef;

using Prism.Events;

namespace LizeriumSteam.Events
{
    public class AppUpdateCompletedEvent : PubSubEvent<StateErrorUpdate> { }
}
