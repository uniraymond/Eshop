using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Application.BackgroundJobs.Interfaces
{
    public interface ILogCleanupJobService
    {
        Task CleanupOldLogsAsync();
    }
}
