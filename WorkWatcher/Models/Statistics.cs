using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkWatcher.Bases;

namespace WorkWatcher.Models
{
    public class Statistics : Model
    {
        private TimeSpan _totalWorkTime;
        public TimeSpan TotalWorkTime
        {
            get => _totalWorkTime;
            set
            {
                _totalWorkTime = value;
                OnPropertyChanged(nameof(TotalWorkTime));
            }
        }
        private TimeSpan _totalDistractionTime;
        public TimeSpan TotalDistractionTime
        {
            get => _totalDistractionTime;
            set
            {
                _totalDistractionTime = value;
                OnPropertyChanged(nameof(TotalDistractionTime));
            }
        }
        private TimeSpan _totalComputerTime;
        public TimeSpan TotalComputerTime
        {
            get => _totalComputerTime;
            set
            {
                _totalComputerTime = value;
                OnPropertyChanged(nameof(TotalComputerTime));
            }
        }

        private int _totalSessionsCount;
        public int TotalSessionsCount
        {
            get => _totalSessionsCount;
            set
            {
                _totalSessionsCount = value;
                OnPropertyChanged(nameof(TotalSessionsCount));
            }
        }

        private int _quotaMetCount;
        public int QuotaMetCount
        {
            get => _quotaMetCount;
            set
            {
                _quotaMetCount = value;
                OnPropertyChanged(nameof(QuotaMetCount));
            }
        }

        private int _punishmentCount;
        public int PunishmentCount
        {
            get => _punishmentCount;
            set
            {
                _punishmentCount = value;
                OnPropertyChanged(nameof(PunishmentCount));
            }
        }
    }
}
