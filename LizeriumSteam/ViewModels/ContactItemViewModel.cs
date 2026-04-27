/*
 * Author: Nikolay Dvurechensky
 * Site: https://dvurechensky.pro/
 * Gmail: dvurechenskysoft@gmail.com
 * Last Updated: 27 апреля 2026 10:02:12
 * Version: 1.0.34
 */

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

using LizeriumSteam.Models.Settings;

using Prism.Commands;

namespace LizeriumSteam.ViewModels
{
    public class ContactItemViewModel
    {
        public string Name => (currentLang == "ru") ? contact.Name.ru : contact.Name.en;
        public string InfoName => (currentLang == "ru") ? contact.Info.Name.ru : contact.Info.Name.en;

        public ObservableCollection<ContactInfoViewModel> SubContacts { get; }
            = new ObservableCollection<ContactInfoViewModel>();

        private readonly ContactItemModel contact;
        private readonly string currentLang;

        public ContactItemViewModel(ContactItemModel contactItem, string lang)
        {
            contact = contactItem;
            currentLang = lang;
            if (contact.Info != null)
            {
                SubContacts.Add(new ContactInfoViewModel(contact.Info, lang));
            }
        }

        public class ContactInfoViewModel
        {
            public string InfoName { get; }
            public ObservableCollection<ContactResourceViewModel> Resources { get; }
                = new ObservableCollection<ContactResourceViewModel>();

            public ContactInfoViewModel(ContactInfo info, string lang)
            {
                InfoName = (lang == "ru") ? info.Name.ru : info.Name.en;

                if (info.Resources != null)
                {
                    foreach (var r in info.Resources)
                    {
                        Resources.Add(new ContactResourceViewModel(r, lang));
                    }
                }
            }
        }

        public class ContactResourceViewModel
        {
            public string Name { get; }
            public string IconPathData { get; }
            public ObservableCollection<string> Urls { get; } = new ObservableCollection<string>();
            public ICommand OpenUrlCommand { get; }

            public ContactResourceViewModel(ContactResource resource, string lang)
            {
                Name = (lang == "ru") ? resource.Name.ru : resource.Name.en;
                IconPathData = resource.Icon;

                foreach (var url in resource.Urls)
                    Urls.Add(url);

                OpenUrlCommand = new DelegateCommand(() =>
                {
                    if (Urls.Count > 0)
                    {
                        try
                        {
                            var psi = new ProcessStartInfo
                            {
                                FileName = Urls[0],
                                UseShellExecute = true
                            };
                            Process.Start(psi);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                });
            }
        }
    }
}
