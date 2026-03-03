using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkWatcher.Models
{
    public class MonitoringSession
    {
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan TotalWorkTime { get; set; }
        public TimeSpan TotalDistractionTime { get; set; }
        public TimeSpan TotalComputerTime { get; set; }
        public Quota SessionQuota { get; set; }
        public Dictionary<string, TimeSpan> ProgramUsage { get; set; } = new();
    }
}
