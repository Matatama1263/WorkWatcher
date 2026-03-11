using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using WorkWatcher.Models;

namespace WorkWatcher.Services
{
    public static class DataStorageService
    {
        private static readonly string _dataDirectory;
        private static readonly string _sessionsFile;
        private static readonly string _settingsFile;
        static DataStorageService()
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

        public static void SaveSession(MonitoringSession session)
        {
            var sessions = LoadAllSessions();
            sessions.Add(session);

            var json = JsonSerializer.Serialize(sessions, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(_sessionsFile, json);
        }

        public static List<MonitoringSession> LoadAllSessions()
        {
            if (!File.Exists(_sessionsFile))
            {
                return new List<MonitoringSession>();
            }

            var json = File.ReadAllText(_sessionsFile);
            return JsonSerializer.Deserialize<List<MonitoringSession>>(json)
                   ?? new List<MonitoringSession>();
        }

        public static void SaveSettings(AppSettings settings)
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(_settingsFile, json);
        }

        public static AppSettings LoadSettings()
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
        public List<string> MonitoredPrograms { get; set; } = new();
        public List<DistractionProgramInfo> DistractionPrograms { get; set; } = new();
        public Quota DailyQuota { get; set; } = new();
    }
}