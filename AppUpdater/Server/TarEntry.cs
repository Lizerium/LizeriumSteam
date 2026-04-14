/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 14 апреля 2026 12:40:18
 * Version: 1.0.19
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
