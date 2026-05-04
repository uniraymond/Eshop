using Eshop.Application.BackgroundJobs.Interfaces;
using Hangfire;

namespace Eshop.API.Extensions
{
    public static class HangfireJobExtensions
    {
        public static void RegisterRecurringJobs(this WebApplication app)
        {
            RecurringJob.AddOrUpdate<IOrderBackgroundJobService>(
                "cancel-expired-pending-orders",
                job => job.CancelExpiredPendingOrdersAsync(),
                Cron.Minutely
            );

            RecurringJob.AddOrUpdate<ILogCleanupJobService>(
                "log-cleanup-job",
                job => job.CleanupOldLogsAsync(),
                Cron.Daily
            );
        }
    }
}
