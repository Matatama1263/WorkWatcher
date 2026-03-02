using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkWatcher.Models
{
    public enum ProgramCategory
    {
        Distraction,  // 딴짓 프로그램
        Work          // 작업 프로그램
    }

    public class ProgramInfo
    {
        // 프로그램 위치
        public string Name { get; set; }
        // 프로그램 이름 (예: "Google Chrome", "Visual Studio")
        public string ProcessName { get; set; }
        public ProgramCategory Category { get; set; }
        public bool StrictMonitoring { get; set; }  // 강한 감시 여부
        public TimeSpan TotalActiveTime { get; set; }
    }
}
