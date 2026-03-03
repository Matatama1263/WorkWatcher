using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkWatcher.Models
{
    public class Quota
    {
        public TimeSpan RequiredWorkTime { get; set; }  // 할당량
        public TimeSpan PunishmentThreshold { get; set; }  // 처벌 시간
        public bool QuotaMet { get; set; }
        public bool PunishmentActive { get; set; }
        public bool Punishmented { get; set; }


        public void InitializeQuota(TimeSpan requiredWorkTime, TimeSpan punishmentThreshold)
        {
            RequiredWorkTime = requiredWorkTime;
            PunishmentThreshold = punishmentThreshold;
            QuotaMet = false;
            PunishmentActive = false;
            Punishmented = false;
        }
    }
}
