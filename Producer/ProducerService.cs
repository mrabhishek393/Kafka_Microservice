using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace Producer
{

    /// <summary>
    /// A class to perform producer operations.
    /// </summary>
    public class ProducerService : IProducerService, IDisposable
    {
        private readonly IProducer<Null, string>? _producer; //producer

        public ProducerService(IConfiguration config)
        {
            // Initializing producer config from user secrets.
            ProducerConfig producerconfig =
                new ProducerConfig { BootstrapServers = config["Kafka:ProducerSettings:BootstrapServers"] };

            // Building producer
            _producer = new ProducerBuilder<Null, string>(producerconfig).Build();
        }

        /// <summary>
        /// Publishes message to the specified topic asynchronously.
        /// </summary>
        /// <param name="topic"> Topic to which message will be published</param>
        /// <param name="message"> The message that's need to be published</param>
        /// <returns>Task</returns>
        public async Task WriteMessage(string topic, string message)
        {
            try
            {
                var delivery_report = await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message });

            }
            catch (ProduceException<Null, string>)
            {
                throw;
            }

        }

        public void Dispose()
        {
            _producer.Flush();
            _producer.Dispose();
        }
    }
}
