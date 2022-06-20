
namespace Producer
{

    /// <summary>
    /// Interface for Producer Service
    /// </summary>
    public interface IProducerService
    {
        Task WriteMessage(string topic, string message);
    }
}