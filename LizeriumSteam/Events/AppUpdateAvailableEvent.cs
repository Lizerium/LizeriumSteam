/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 03 апреля 2026 11:47:22
 * Version: 1.0.8
 */

using Prism.Events;

namespace LizeriumSteam.Events
{
    public class AppUpdateAvailableEvent : PubSubEvent<string> { }
}
