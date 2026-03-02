using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkWatcher.Models
{
    public class MonitoringSession
    {
        public Guid SessionId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan TotalWorkTime { get; set; }
        public TimeSpan TotalDistractionTime { get; set; }
        public TimeSpan TotalComputerTime { get; set; }
        public bool QuotaMet { get; set; }
        public Dictionary<string, TimeSpan> ProgramUsage { get; set; } = new();
    }
}
