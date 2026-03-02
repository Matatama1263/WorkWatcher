using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkWatcher.Services
{
    public class TimeTrackerService
    {

        public TimeTrackerService()
        {
            timer = new Timer(TimerCallback);
        }

        public void TimerCallback(object state)
        {
            
        }

        Timer timer;

        public void StartTracking()
        {
            timer.Change(0, 100);
        }

        public void StopTracking()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }
}
