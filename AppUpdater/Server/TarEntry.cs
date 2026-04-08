/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 08 апреля 2026 14:44:03
 * Version: 1.0.13
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
