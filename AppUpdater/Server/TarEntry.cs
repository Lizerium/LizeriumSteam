/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 30 июля 2026 07:10:26
 * Version: 1.0.126
 */

namespace AppUpdater.Server
{
    public class TarCustomEntry
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public byte[] Content { get; set; }
    }
}
