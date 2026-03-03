using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkWatcher.Models
{
    public class ProgramInfo
    {
        // 프로그램 위치
        public string Name { get; set; }
        // 프로그램 이름 (예: "Google Chrome", "Visual Studio")
        public string ProcessName { get; set; }
        public TimeSpan TotalActiveTime { get; set; }
    }

    public class DistractionProgramInfo : ProgramInfo
    {
        public bool StrictMonitoring { get; set; }  // 강한 감시 여부
    }
}
