/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 29 апреля 2026 07:13:25
 * Version: 1.0.36
 */

using System.Collections.Generic;

namespace LizeriumSteam.Models.Settings
{
    public sealed class LocalizedString
    {
        public string ru { get; set; }
        public string en { get; set; }
    }

    public sealed class ContactItemModel
    {
        public LocalizedString Name { get; set; }
        public ContactInfo Info { get; set; }
    }

    public sealed class ContactInfo
    {
        public LocalizedString Name { get; set; }
        public List<ContactResource> Resources { get; set; }
    }

    public sealed class ContactResource
    {
        public LocalizedString Name { get; set; }
        public string Icon { get; set; }
        public List<string> Urls { get; set; }
    }

    public sealed class AppGlobalSettings
    {
        public string Version { get; set; } = "1.0.0";
        public string Language { get; set; }
        public List<ContactItemModel> Contacts { get; set; }
    }
}
