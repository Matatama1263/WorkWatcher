using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkWatcher.Bases;

namespace WorkWatcher.Models
{
    public class MonitoringSession : Model
    {

        public MonitoringSession()
        {
            _sessionQuota = new Quota();
            Initialize(); // 생성자에서 세션 초기화
        }

        public void Initialize()
        {
            EndTime = null; // 세션이 시작될 때 종료 시간 초기화
            TotalWorkTime = TimeSpan.Zero;
            TotalDistractionTime = TimeSpan.Zero;
            TotalComputerTime = TimeSpan.Zero;
            ProgramUsage.Clear();
        }

        private DateTime _startTime;
        public DateTime StartTime
        { 
            get => _startTime; 
            set
            {
                Initialize(); // 시작 시간이 설정될 때 세션 초기화
                _startTime = value;
                OnPropertyChanged(nameof(StartTime));
            }
        }

        private DateTime? _endTime;
        public DateTime? EndTime
        { 
            get => _endTime; 
            set
            {
                _endTime = value;
                OnPropertyChanged(nameof(EndTime));
            }
        }

        private TimeSpan _totalWorkTime;
        public TimeSpan TotalWorkTime
        { 
            get => _totalWorkTime; 
            set
            {
                _totalWorkTime = value;
                RemainQuotaTime = SessionQuota.QuotaTime - TotalDistractionTime; // 남은 쿼터 시간 업데이트
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
                RemainDistractTime = SessionQuota.PunishmentThreshold - TotalDistractionTime; // 남은 방해 시간 업데이트
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

        private Quota _sessionQuota = new();
        public Quota SessionQuota
        {
            get => _sessionQuota; 
            set
            {
                _sessionQuota = value;
                OnPropertyChanged(nameof(SessionQuota));
            }
        }

        private TimeSpan _remainQuotaTime;
        public TimeSpan RemainQuotaTime
        {
            get => _remainQuotaTime; 
            set
            {
                _remainQuotaTime = value;
                OnPropertyChanged(nameof(RemainQuotaTime));
            }
        }

        private TimeSpan _remainDistractTime;
        public TimeSpan RemainDistractTime
        {
            get => _remainDistractTime; 
            set
            {
                _remainDistractTime = value;
                OnPropertyChanged(nameof(RemainDistractTime));
            }
        }

        private Dictionary<string, TimeSpan> _programUsage = new();
        public Dictionary<string, TimeSpan> ProgramUsage
        {
            get => _programUsage; 
            set
            {
                _programUsage = value;
                OnPropertyChanged(nameof(ProgramUsage));
            }
        }
    }
}
