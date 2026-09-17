/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 17 сентября 2026 06:59:56
 * Version: 1.0.174
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
