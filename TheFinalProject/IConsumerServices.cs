
namespace ConsumerApp
{

    /// <summary>
    /// Interface for Database Services
    /// </summary>
    public interface IConsumerServices
    {
        void Dispose();
        Task StartConsumerLoop(CancellationToken stoppingToken);
    }
}