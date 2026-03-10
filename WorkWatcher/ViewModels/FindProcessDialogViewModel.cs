using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkWatcher.Bases;
using System.Diagnostics;

namespace WorkWatcher.ViewModels
{
    public class FindProcessDialogViewModel : Model
    {
        
        public FindProcessDialogViewModel()
        {
            Process[] processes = Process.GetProcesses();

            List<string> windows = new List<string>();

            foreach (var process in processes)
            {
                if (!string.IsNullOrEmpty(process.MainWindowTitle))
                {
                    windows.Add(process.ProcessName);
                }
            }

            ProcessNames = windows.Distinct().ToList();
        }

        private List<string> _processNames;
        public List<string> ProcessNames
        {
            get => _processNames;
            set
            {
                _processNames = value;
                OnPropertyChanged(nameof(ProcessNames));
            }
        }
    }
}
