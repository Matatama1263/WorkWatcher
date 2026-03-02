using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using WorkWatcher.Models;

namespace WorkWatcher.Services
{
    public class DataStorageService
    {
        private readonly string _dataDirectory;
        private readonly string _sessionsFile;
        private readonly string _settingsFile;

        public DataStorageService()
        {
            _dataDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "WorkWatcher"
            );
            _sessionsFile = Path.Combine(_dataDirectory, "sessions.json");
            _settingsFile = Path.Combine(_dataDirectory, "settings.json");

            if (!Directory.Exists(_dataDirectory))
            {
                Directory.CreateDirectory(_dataDirectory);
            }
        }

        public void SaveSession(MonitoringSession session)
        {
            var sessions = LoadAllSessions();
            sessions.Add(session);

            var json = JsonSerializer.Serialize(sessions, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(_sessionsFile, json);
        }

        public List<MonitoringSession> LoadAllSessions()
        {
            if (!File.Exists(_sessionsFile))
            {
                return new List<MonitoringSession>();
            }

            var json = File.ReadAllText(_sessionsFile);
            return JsonSerializer.Deserialize<List<MonitoringSession>>(json)
                   ?? new List<MonitoringSession>();
        }

        public void SaveSettings(AppSettings settings)
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(_settingsFile, json);
        }

        public AppSettings LoadSettings()
        {
            if (!File.Exists(_settingsFile))
            {
                return new AppSettings();
            }

            var json = File.ReadAllText(_settingsFile);
            return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }
    }

    public class AppSettings
    {
        public List<ProgramInfo> MonitoredPrograms { get; set; } = new();
        public Quota DailyQuota { get; set; } = new();
        public bool StartWithWindows { get; set; }
    }
}