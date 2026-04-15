/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 15 апреля 2026 07:05:07
 * Version: 1.0.22
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
