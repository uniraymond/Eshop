namespace Eshop.Application.BackgroundJobs.Interfaces
{
    public interface IOutboxProcessor
    {
        Task ProcessAsync();
    }
}
