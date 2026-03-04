using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkWatcher.Bases;

namespace WorkWatcher.Models
{
    public class ProgramInfo : Model
    {

        // 프로그램 위치
        private string _name;
        public string Name
        {
            get => _name; 
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        // 프로그램 이름 (예: "Google Chrome", "Visual Studio")
        private string _processName;
        public string ProcessName
        {
            get => _processName; 
            set
            {
                _processName = value;
                OnPropertyChanged(nameof(ProcessName));
            }
        }

        private TimeSpan _totalActiveTime;

        public TimeSpan TotalActiveTime
        {
            get => _totalActiveTime; 
            set
            {
                _totalActiveTime = value;
                OnPropertyChanged(nameof(TotalActiveTime));
            }
        }
    }

    public class DistractionProgramInfo : ProgramInfo
    {
        // 강한 감시 여부
        private bool _strictMonitoring;
        public bool StrictMonitoring
        {
            get => _strictMonitoring; 
            set
            {
                _strictMonitoring = value;
                OnPropertyChanged(nameof(StrictMonitoring));
            }
        }
    }
}
