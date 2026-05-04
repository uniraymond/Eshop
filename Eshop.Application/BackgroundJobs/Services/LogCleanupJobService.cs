using Eshop.Application.BackgroundJobs.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.BackgroundJobs.Services
{
    public class LogCleanupJobService : ILogCleanupJobService
    {
        private readonly ILogger _logger;

        public LogCleanupJobService(ILogger<LogCleanupJobService> logger)
        {
            _logger = logger;
        }

        public Task CleanupOldLogsAsync()
        {
            var logsPath = Path.Combine(Directory.GetCurrentDirectory(), "logs");

            if (!Directory.Exists(logsPath))
            {
                _logger.LogInformation("Logs directory does not exist at path: {LogsPath}. No logs to clean up.", logsPath);
                return Task.CompletedTask;
            }

            var cutoffDate = DateTime.UtcNow.AddDays(-14);

            var files = Directory.GetFiles(logsPath, "*.log", SearchOption.TopDirectoryOnly);

            var deletedCount = 0;

            foreach (var file in files)
            {
                var lastWriteTimeUtc = File.GetLastWriteTimeUtc(file);

                if (lastWriteTimeUtc < cutoffDate)
                {
                    File.Delete(file);
                    deletedCount++;
                }
            }

            _logger.LogInformation("Log cleanup completed. Deleted {DeletedCount} log files older than {CutoffDate}.", deletedCount, cutoffDate);

            return Task.CompletedTask;
        }
    }
}
