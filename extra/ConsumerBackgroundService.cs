using Confluent.Kafka;
using Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace extra
{
    public class ConsumerBackgroundService : BackgroundService
    {

        private readonly string _topic;
        private readonly IConsumer<Ignore, string> _consumer;
        IDatabaseServices _databaseServices;
        public ConsumerBackgroundService(IConfiguration config,IDatabaseServices databaseServices)
        {
            //Console.WriteLine(await databaseServices.GetbyIdAsync(2));

            //_databaseServices = databaseServices;
            Console.WriteLine(config["Kafka:Topic"]);
            //ConsumerConfig consumerconfig = new ConsumerConfig
            //{
            //    GroupId = config["Kafka:ConsumerSettings:GroupId"],
            //    BootstrapServers = config["Kafka:ConsumerSettings:BootstrapServers"],
            //    AutoOffsetReset = AutoOffsetReset.Latest
            //};
            //_consumer = new ConsumerBuilder<Ignore, string>(consumerconfig).Build();
            //_topic = config["Kafka:Topic"];
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //await Task.Run(() => helper(stoppingToken));
            //var mymodel = await _databaseServices.GetbyIdAsync(2);
            //Console.WriteLine(mymodel.Name);
            Console.WriteLine("d");
        }

        //private void helper(CancellationToken stoppingToken)
        //{
        //    //while (!stoppingToken.IsCancellationRequested) Console.WriteLine("service");
        //    _consumer.Subscribe(_topic);
        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        try
        //        {
        //            var cr = _consumer.Consume(stoppingToken);

        //            // Handle message...
        //            Console.WriteLine(cr.Message.Value);
        //        }
        //        catch (OperationCanceledException)
        //        {
        //            break;
        //        }
        //        catch (ConsumeException e)
        //        {
        //            // Consumer errors should generally be ignored (or logged) unless fatal.
        //            Console.WriteLine($"Consume error: {e.Error.Reason}");

        //            if (e.Error.IsFatal)
        //            {
        //                break;
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine($"Unexpected error: {e}");
        //            break;
        //        }
        //    }

        //}
        //public override void Dispose()
        //{
        //    _consumer.Close(); // Commit offsets and leave the group cleanly.

        //    _consumer.Dispose();

        //    base.Dispose();
        //}
    }
}
