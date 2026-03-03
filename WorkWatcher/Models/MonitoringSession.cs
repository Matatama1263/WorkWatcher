using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkWatcher.Models
{
    public class MonitoringSession
    {
        private DateTime _startTime;
        public DateTime StartTime
        { 
            get => _startTime; 
            set
            {
                _startTime = value;
                _endTime = null; // 세션이 시작될 때 종료 시간 초기화
                _totalWorkTime = TimeSpan.Zero;
                _totalDistractionTime = TimeSpan.Zero;
                _totalComputerTime = TimeSpan.Zero;
                _programUsage.Clear();
            }
        }
        private DateTime? _endTime;
        public DateTime? EndTime
        { 
            get => _endTime; 
            set{ _endTime = value; }
        }

        private TimeSpan _totalWorkTime;
        public TimeSpan TotalWorkTime
        { 
            get => _totalWorkTime; 
            set { _totalWorkTime = value; }
        }

        private TimeSpan _totalDistractionTime;
        public TimeSpan TotalDistractionTime
        {
            get => _totalDistractionTime; 
            set { _totalDistractionTime = value; }
        }

        private TimeSpan _totalComputerTime;
        public TimeSpan TotalComputerTime
        {
            get => _totalComputerTime; 
            set { _totalComputerTime = value; }
        }

        private Quota _sessionQuota = new();
        public Quota SessionQuota
        {
            get => _sessionQuota; 
            set { _sessionQuota = value; }
        }

        private Dictionary<string, TimeSpan> _programUsage = new();
        public Dictionary<string, TimeSpan> ProgramUsage
        {
            get => _programUsage; 
            set { _programUsage = value; }
        }
    }
}
