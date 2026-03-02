using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkWatcher.Models;

namespace WorkWatcher.Services
{
    public class SessionManagementService
    {
        MonitoringSession currentSession;
        Quota currentQuota;
        TimeTrackerService timeTracker;
        ProcessMonitorService processMonitor;

        public SessionManagementService()
        {
            timeTracker = new TimeTrackerService();
            processMonitor = new ProcessMonitorService();
            currentQuota = new Quota();
            currentSession = new MonitoringSession();
        }

        public void StartNewSession(Quota quota)
        {
            currentSession = new MonitoringSession
            {
                SessionId = Guid.NewGuid(),
                StartTime = DateTime.Now,
                QuotaMet = false
            };
            currentQuota = quota;
            timeTracker.StartTracking();
            processMonitor.StartMonitoring();
        }
    }
}
