using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkWatcher.Services;

namespace WorkWatcher.ViewModels
{
    public class SettingsViewModel
    {
        AppSettings AppSettings { get; set; }
        public SettingsViewModel(AppSettings appSettings)
        {
            AppSettings = appSettings;
        }
    }
}
