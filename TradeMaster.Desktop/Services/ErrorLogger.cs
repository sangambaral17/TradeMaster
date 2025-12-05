using System;
using System.IO;
using System.Text;

namespace TradeMaster.Desktop.Services;

/// <summary>
/// Provides file-based error logging functionality for the application.
/// Logs are stored in the application's data directory with automatic rotation.
/// </summary>
public class ErrorLogger
{
    private static readonly string LogDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Walsong",
        "TradeMaster",
        "Logs"
    );

    private static readonly object _lockObject = new();

    static ErrorLogger()
    {
        // Ensure log directory exists
        if (!Directory.Exists(LogDirectory))
        {
            Directory.CreateDirectory(LogDirectory);
        }

        // Clean up old logs (keep last 7 days)
        CleanupOldLogs();
    }

    /// <summary>
    /// Logs an error message with exception details.
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="exception">The exception that occurred</param>
    /// <param name="context">Additional context information</param>
    public static void LogError(string message, Exception? exception = null, string? context = null)
    {
        try
        {
            var logEntry = BuildLogEntry(message, exception, context, "ERROR");
            WriteToFile(logEntry);
        }
        catch
        {
            // Silently fail - don't throw exceptions from logger
        }
    }

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">The warning message</param>
    /// <param name="context">Additional context information</param>
    public static void LogWarning(string message, string? context = null)
    {
        try
        {
            var logEntry = BuildLogEntry(message, null, context, "WARNING");
            WriteToFile(logEntry);
        }
        catch
        {
            // Silently fail
        }
    }

    /// <summary>
    /// Logs an informational message.
    /// </summary>
    /// <param name="message">The information message</param>
    /// <param name="context">Additional context information</param>
    public static void LogInfo(string message, string? context = null)
    {
        try
        {
            var logEntry = BuildLogEntry(message, null, context, "INFO");
            WriteToFile(logEntry);
        }
        catch
        {
            // Silently fail
        }
    }

    /// <summary>
    /// Gets the path to today's log file.
    /// </summary>
    public static string GetTodayLogFilePath()
    {
        var fileName = $"TradeMaster_{DateTime.Now:yyyy-MM-dd}.log";
        return Path.Combine(LogDirectory, fileName);
    }

    /// <summary>
    /// Gets all log files sorted by date (newest first).
    /// </summary>
    public static FileInfo[] GetAllLogFiles()
    {
        var directory = new DirectoryInfo(LogDirectory);
        return directory.GetFiles("TradeMaster_*.log")
            .OrderByDescending(f => f.CreationTime)
            .ToArray();
    }

    private static string BuildLogEntry(string message, Exception? exception, string? context, string level)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level}]");
        sb.AppendLine($"Message: {message}");

        if (!string.IsNullOrWhiteSpace(context))
        {
            sb.AppendLine($"Context: {context}");
        }

        if (exception != null)
        {
            sb.AppendLine($"Exception Type: {exception.GetType().FullName}");
            sb.AppendLine($"Exception Message: {exception.Message}");
            sb.AppendLine($"Stack Trace:");
            sb.AppendLine(exception.StackTrace);

            // Log inner exceptions
            var innerException = exception.InnerException;
            var innerLevel = 1;
            while (innerException != null)
            {
                sb.AppendLine($"Inner Exception {innerLevel}: {innerException.GetType().FullName}");
                sb.AppendLine($"Message: {innerException.Message}");
                sb.AppendLine($"Stack Trace:");
                sb.AppendLine(innerException.StackTrace);
                innerException = innerException.InnerException;
                innerLevel++;
            }
        }

        sb.AppendLine(new string('-', 80));
        return sb.ToString();
    }

    private static void WriteToFile(string logEntry)
    {
        var logFilePath = GetTodayLogFilePath();

        lock (_lockObject)
        {
            File.AppendAllText(logFilePath, logEntry);
        }
    }

    private static void CleanupOldLogs()
    {
        try
        {
            var cutoffDate = DateTime.Now.AddDays(-7);
            var directory = new DirectoryInfo(LogDirectory);

            foreach (var file in directory.GetFiles("TradeMaster_*.log"))
            {
                if (file.CreationTime < cutoffDate)
                {
                    file.Delete();
                }
            }
        }
        catch
        {
            // Silently fail
        }
    }
}
