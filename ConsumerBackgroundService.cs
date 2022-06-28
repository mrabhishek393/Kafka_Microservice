using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsumerApp
{

    /// <summary>
    /// A background service class which runs consumer and perform operations in the background all the time.
    /// </summary>
    public class ConsumerBackgroundService : BackgroundService
    {

        private readonly IConsumerServices _consumerServices; // Consumer services
        private readonly ILogger<ConsumerBackgroundService> _logger;

        public ConsumerBackgroundService(IConsumerServices consumerServices, ILogger<ConsumerBackgroundService> logger)
        {
            _consumerServices = consumerServices;
            _logger = logger;
        }

        /// <summary>
        /// Trigger a consumer loop
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting Consumer Background service");

            try
            {
                await _consumerServices.StartConsumerLoop(stoppingToken);
            }
            catch (OperationCanceledException e)
            {
                _logger.LogWarning("Cancelling Operation:  " + e.ToString());
            }
            catch (ConsumeException ex)
            {
                _logger.LogCritical("Fatal Consume Error --> Shutting down microservice: " + ex.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Fatal Error --> Shutting down microservice: " + ex.ToString());
            }
        }


        /// <summary>
        /// Dispose the consumer
        /// </summary>
        public override void Dispose()
        {
            _logger.LogInformation("Consumer Service stopped. Disposing Consumer");

            _consumerServices.Dispose();
            base.Dispose();


        }
    }
}
