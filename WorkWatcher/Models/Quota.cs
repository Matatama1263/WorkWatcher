using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkWatcher.Bases;

namespace WorkWatcher.Models
{
    public class Quota : Model
    {

        private TimeSpan _requiredWorkTime;
        public TimeSpan QuotaTime
        {
            get => _requiredWorkTime; 
            set
            {
                _requiredWorkTime = value;
                OnPropertyChanged(nameof(QuotaTime));
            }
        }

        private TimeSpan _punishmentThreshold;
        public TimeSpan PunishmentThreshold
        {
            get => _punishmentThreshold; 
            set
            {
                _punishmentThreshold = value;
                OnPropertyChanged(nameof(PunishmentThreshold));
            }
        }

        private bool _quotaMet;
        public bool QuotaMet
        {
            get => _quotaMet; 
            set
            {
                _quotaMet = value;
                OnPropertyChanged(nameof(QuotaMet));
            }
        }

        private bool _punishmentActive;
        public bool PunishmentActive
        {
            get => _punishmentActive;
            set
            {
                _punishmentActive = value;
                OnPropertyChanged(nameof(PunishmentActive));
            }
        }

        private bool _punishmented;
        public bool Punishmented
        {
            get => _punishmented; 
            set
            {
                _punishmented = value;
                OnPropertyChanged(nameof(Punishmented));
            }
        }


        public void InitializeQuota(TimeSpan requiredWorkTime, TimeSpan punishmentThreshold)
        {
            QuotaTime = requiredWorkTime;
            PunishmentThreshold = punishmentThreshold;
            QuotaMet = false;
            PunishmentActive = false;
            Punishmented = false;
        }
    }
}
