using System.IO;
using System.IO.Compression;

namespace TradeMaster.Desktop.Services
{
    /// <summary>
    /// Service for database backup and restore operations.
    /// </summary>
    public class BackupService
    {
        private readonly SettingsService _settingsService;
        private readonly string _databasePath;

        public BackupService(SettingsService settingsService)
        {
            _settingsService = settingsService;
            _databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "trademaster.db");
        }

        /// <summary>
        /// Creates a backup of the database.
        /// </summary>
        /// <returns>Path to the created backup file.</returns>
        public async Task<string> CreateBackupAsync()
        {
            var settings = _settingsService.Settings;
            var backupFolder = string.IsNullOrEmpty(settings.BackupLocation)
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TradeMasterBackups")
                : settings.BackupLocation;

            // Ensure backup folder exists
            if (!Directory.Exists(backupFolder))
            {
                Directory.CreateDirectory(backupFolder);
            }

            // Generate backup filename with timestamp
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var backupFileName = $"TradeMaster_Backup_{timestamp}.db";
            var backupPath = Path.Combine(backupFolder, backupFileName);

            // Copy database file
            await Task.Run(() =>
            {
                File.Copy(_databasePath, backupPath, overwrite: true);
            });

            // Update last backup date in settings
            _settingsService.UpdateSetting(s => s.LastBackupDate = DateTime.Now);

            // Clean up old backups
            await CleanupOldBackupsAsync(backupFolder, settings.MaxBackupsToKeep);

            return backupPath;
        }

        /// <summary>
        /// Creates a compressed backup of the database.
        /// </summary>
        public async Task<string> CreateCompressedBackupAsync()
        {
            var settings = _settingsService.Settings;
            var backupFolder = string.IsNullOrEmpty(settings.BackupLocation)
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TradeMasterBackups")
                : settings.BackupLocation;

            if (!Directory.Exists(backupFolder))
            {
                Directory.CreateDirectory(backupFolder);
            }

            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var backupFileName = $"TradeMaster_Backup_{timestamp}.zip";
            var backupPath = Path.Combine(backupFolder, backupFileName);

            await Task.Run(() =>
            {
                using var archive = ZipFile.Open(backupPath, ZipArchiveMode.Create);
                archive.CreateEntryFromFile(_databasePath, "trademaster.db", CompressionLevel.Optimal);
            });

            _settingsService.UpdateSetting(s => s.LastBackupDate = DateTime.Now);

            return backupPath;
        }

        /// <summary>
        /// Restores database from a backup file.
        /// </summary>
        /// <param name="backupPath">Path to the backup file.</param>
        public async Task RestoreBackupAsync(string backupPath)
        {
            if (!File.Exists(backupPath))
            {
                throw new FileNotFoundException("Backup file not found.", backupPath);
            }

            await Task.Run(() =>
            {
                // Create a safety backup before restoring
                var safetyBackupPath = _databasePath + ".safety";
                File.Copy(_databasePath, safetyBackupPath, overwrite: true);

                try
                {
                    if (backupPath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                    {
                        // Extract from zip
                        using var archive = ZipFile.OpenRead(backupPath);
                        var entry = archive.GetEntry("trademaster.db");
                        if (entry != null)
                        {
                            entry.ExtractToFile(_databasePath, overwrite: true);
                        }
                        else
                        {
                            throw new InvalidOperationException("Backup archive does not contain the database file.");
                        }
                    }
                    else
                    {
                        // Direct copy
                        File.Copy(backupPath, _databasePath, overwrite: true);
                    }

                    // Remove safety backup on success
                    File.Delete(safetyBackupPath);
                }
                catch
                {
                    // Restore safety backup on failure
                    if (File.Exists(safetyBackupPath))
                    {
                        File.Copy(safetyBackupPath, _databasePath, overwrite: true);
                        File.Delete(safetyBackupPath);
                    }
                    throw;
                }
            });
        }

        /// <summary>
        /// Gets list of available backup files.
        /// </summary>
        public List<BackupInfo> GetAvailableBackups()
        {
            var settings = _settingsService.Settings;
            var backupFolder = string.IsNullOrEmpty(settings.BackupLocation)
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TradeMasterBackups")
                : settings.BackupLocation;

            if (!Directory.Exists(backupFolder))
            {
                return new List<BackupInfo>();
            }

            var backups = new List<BackupInfo>();
            
            var files = Directory.GetFiles(backupFolder, "TradeMaster_Backup_*.*")
                .Where(f => f.EndsWith(".db") || f.EndsWith(".zip"));

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                backups.Add(new BackupInfo
                {
                    FilePath = file,
                    FileName = fileInfo.Name,
                    CreatedDate = fileInfo.CreationTime,
                    SizeBytes = fileInfo.Length,
                    IsCompressed = file.EndsWith(".zip")
                });
            }

            return backups.OrderByDescending(b => b.CreatedDate).ToList();
        }

        /// <summary>
        /// Removes old backups keeping only the specified number.
        /// </summary>
        private async Task CleanupOldBackupsAsync(string backupFolder, int maxBackupsToKeep)
        {
            await Task.Run(() =>
            {
                var backups = Directory.GetFiles(backupFolder, "TradeMaster_Backup_*.*")
                    .Where(f => f.EndsWith(".db") || f.EndsWith(".zip"))
                    .Select(f => new FileInfo(f))
                    .OrderByDescending(f => f.CreationTime)
                    .Skip(maxBackupsToKeep)
                    .ToList();

                foreach (var backup in backups)
                {
                    try
                    {
                        backup.Delete();
                    }
                    catch
                    {
                        // Ignore deletion errors
                    }
                }
            });
        }

        /// <summary>
        /// Checks if automatic backup is due.
        /// </summary>
        public bool IsBackupDue()
        {
            var settings = _settingsService.Settings;
            if (!settings.AutoBackupEnabled || settings.LastBackupDate == null)
            {
                return settings.AutoBackupEnabled;
            }

            var hoursSinceLastBackup = (DateTime.Now - settings.LastBackupDate.Value).TotalHours;
            return hoursSinceLastBackup >= settings.BackupIntervalHours;
        }
    }

    public class BackupInfo
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public long SizeBytes { get; set; }
        public bool IsCompressed { get; set; }

        public string SizeFormatted => SizeBytes < 1024 * 1024
            ? $"{SizeBytes / 1024.0:F1} KB"
            : $"{SizeBytes / (1024.0 * 1024.0):F1} MB";
    }
}
