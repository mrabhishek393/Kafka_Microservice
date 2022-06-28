using Confluent.Kafka;
using Database;
using Microsoft.Extensions.Configuration;
using Producer;
using Newtonsoft.Json;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Database.Models;

namespace ConsumerApp
{
    /// <summary>
    /// A class to perform consumer operations.
    /// </summary>
    public class ConsumerServices : IConsumerServices
    {
        private readonly IConsumer<Ignore, string>? _consumer; // consumer 
        private readonly IDatabaseServices? _databaseServices; // Database Service
        private readonly IProducerService? _producerService;   // Producer Service
        private readonly IConfiguration? _config;              // Configuration file           
        private readonly ILogger<ConsumerServices> _logger;
        public ConsumerServices(IConfiguration config,
            IDatabaseServices databaseServices,
            IProducerService producerService,
            ILogger<ConsumerServices> logger)
        {
            //Configuration (user secrets) setup
            _config = config;

            //Consumer Setup
            ConsumerConfig consumerconfig = new ConsumerConfig
            {
                GroupId = config["Kafka:ConsumerSettings:GroupId"],
                BootstrapServers = config["Kafka:ConsumerSettings:BootstrapServers"],
                AutoOffsetReset = AutoOffsetReset.Latest
            };
            _consumer = new ConsumerBuilder<Ignore, string>(consumerconfig).Build();

            //Database Service Setup
            _databaseServices = databaseServices;

            //Producer Service Setup
            _producerService = producerService;

            _logger = logger;
        }


        /// <summary>
        /// Starts a loop which continuously listens to all the messages published on the topic.
        /// </summary>
        public async Task StartConsumerLoop(CancellationToken stoppingToken)
        {

            //subscribe to the topic
            _consumer.Subscribe(_config["Kafka:Topic"]);

            //start the loop
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    stoppingToken.ThrowIfCancellationRequested();

                    //consume the messages published on the topic
                    var consumed_message = _consumer.Consume(stoppingToken);
                    if (consumed_message == null)
                    {
                        _logger.LogWarning("Broker is down: TimeOut Reached");
                        continue;
                    }

                    //Perform the neccessary operations on the message
                    await HandleMessage(consumed_message.Message.Value);

                    //await HandleMessage("{\"DateOfRegistration\":\"2012-10-21T00:00:00+05:30\",\"Status\":0}");

                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (ConsumeException e)
                {
                    _logger.LogWarning("Error while consuming:  " + e.ToString());

                    if (e.Error.IsFatal)
                    {
                        throw;
                    }
                }
                catch (JsonSerializationException ex)
                {
                    _logger.LogError("Consumed Message has incorrect format:  " + ex.ToString());
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError("Duplicate Value entered in Primary key(Id) column:  " + ex.ToString());
                }
                catch (ProduceException<Null, string> ex)
                {
                    _logger.LogError("Exception while producing message: " + ex.ToString());
                }
                catch (Exception)
                {
                    throw;
                }
            }

        }

        /// <summary>
        /// Compares the message with information already present in the database.
        /// Possible Operations: -
        /// 1) Insert the new data: if the Id of the new message is not already present in the database.
        /// 2) Update the old data: if the Id is already present in the database.
        /// 3) publish the data to the further-topic
        /// </summary>
        /// <param name="msg">New data pubished ont the topic</param>
        private async Task HandleMessage(string msg)
        {
            GroupModel? newvalue;
            try
            {
                //convert the new message to the database record type
                newvalue = JsonConvert.DeserializeObject<GroupModel>(msg);
            }
            catch (JsonSerializationException)
            {
                throw;
            }

            using var transaction = _databaseServices._Context.Database.BeginTransaction();
            //get the current record from the database if already present.
            var oldvalue = await _databaseServices.GetbyIdAsync(newvalue.Uuid);

            //if no such record is present in the database then
            if (oldvalue == null)
            {
                try
                {
                    //insert the new data
                    await _databaseServices.InsertAysnc(newvalue);

                    //publish the new message to the further-topic
                    await _producerService.WriteMessage(_config["Kafka:further-topic"], JsonConvert.SerializeObject(newvalue));

                    transaction.Commit();
                }
                catch (DbUpdateException)
                {
                    throw;
                }
                catch (ProduceException<Null, string>)
                {
                    throw;
                }
                catch(Exception)
                {
                    throw;
                }
            }
            else
            {
                bool issame = true;
                foreach (PropertyInfo prop in oldvalue.GetType().GetProperties())
                {
                    var newcolumnvalue = newvalue.GetType().GetProperty(prop.Name).GetValue(newvalue, null);
                    var oldcolumnvalue = oldvalue.GetType().GetProperty(prop.Name).GetValue(oldvalue);
                    if ((newcolumnvalue != null) && !newcolumnvalue.Equals(oldcolumnvalue))
                    {
                        issame = false;
                    }
                    else if (newcolumnvalue == null) newvalue.GetType().GetProperty(prop.Name).SetValue(newvalue, oldcolumnvalue, null);

                }

                //new data published on kafka is redundant information ==> issame=true
                // if issame=false, updation must be done.
                if (!issame)
                {
                    try
                    {
                        //Update the data in the database
                        await _databaseServices.UpdateAsync(oldvalue, newvalue);

                        // ==> publish the updated record in the further-topic
                        await _producerService.WriteMessage(_config["Kafka:further-topic"], JsonConvert.SerializeObject(newvalue));
                        transaction.Commit();
                    }
                    catch (ProduceException<Null, string>)
                    {
                        throw;
                    }
                    catch(Exception)
                    {
                        throw;
                    }
                }

            }
        }


        /// <summary>
        /// Closes and disposes the current instances of the consumer.
        /// </summary>
        public void Dispose()
        {
            _consumer.Close(); // Commit offsets and leave the group cleanly.
            _consumer.Dispose(); // Dispose the consumer

        }
    }
}
