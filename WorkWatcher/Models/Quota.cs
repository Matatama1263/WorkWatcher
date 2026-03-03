using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkWatcher.Models
{
    public class Quota
    {
        private TimeSpan _requiredWorkTime;
        public TimeSpan RequiredWorkTime
        {
            get => _requiredWorkTime; 
            set { _requiredWorkTime = value; }
        }

        private TimeSpan _punishmentThreshold;
        public TimeSpan PunishmentThreshold
        {
            get => _punishmentThreshold; 
            set { _punishmentThreshold = value; }
        }

        private bool _quotaMet;
        public bool QuotaMet
        {
            get => _quotaMet; 
            set { _quotaMet = value; }
        }

        private bool _punishmentActive;
        public bool PunishmentActive
        {
            get => _punishmentActive;
            set { _punishmentActive = value; }
        }

        private bool _punishmented;
        public bool Punishmented
        {
            get => _punishmented; 
            set { _punishmented = value; }
        }


        public void InitializeQuota(TimeSpan requiredWorkTime, TimeSpan punishmentThreshold)
        {
            _requiredWorkTime = requiredWorkTime;
            _punishmentThreshold = punishmentThreshold;
            _quotaMet = false;
            _punishmentActive = false;
            _punishmented = false;
        }
    }
}
