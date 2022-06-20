using API.DTOs;
using AutoMapper;
using Confluent.Kafka;
using Database;
using Database.Models;
using Newtonsoft.Json;
using Producer;
using System.Reflection;
namespace API
{

    /// <summary>
    /// A class to perform API operations.
    /// </summary>
    public class APIServices : IAPIServices
    {
        private readonly IDatabaseServices? _databaseServices; // Database services
        private readonly IProducerService? _producerService;   // Producer services
        private readonly IConfiguration? _config; //configuation file
        private readonly ILogger<APIServices> _logger;
        private readonly IMapper _mapper;

        public APIServices(IConfiguration config,
            IDatabaseServices databaseServices,
            IProducerService producerService,
            ILogger<APIServices> logger,
            IMapper mapper
            )
        {
            _config = config;
            _databaseServices = databaseServices;
            _producerService = producerService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<GroupModel>> GetAllRecords()
        {
            return await _databaseServices.GetAllAsync();
        }
        /// <summary>
        /// get the current record from the database using ID if already present other wise null
        /// </summary>
        public async Task<GroupModel> GetSingleRowById(Guid? id)
        {
            return await _databaseServices.GetbyIdAsync(id);
        }

        /// <summary>
        /// Inserts the new record in the database if not already present otherwise updates the record.
        /// And publishes it to further-topic.
        /// </summary>
        public async Task<GroupModel> InsertAsync(AddDataDTO newvalue)
        {
            using (var transaction = _databaseServices._Context.Database.BeginTransaction())
            {
                try
                {

                    //GroupModel postobject = new GroupModel
                    //{
                    //    Id = Guid.NewGuid(),
                    //    Name = newvalue.Name,
                    //    Entitlements = ((newvalue.Entitlements == null) ? new List<Guid>() : newvalue.Entitlements),
                    //    CreatedAt = DateTime.Now
                    //};

                    GroupModel postobject = _mapper.Map<GroupModel>(newvalue);

                    //insert the new data
                    await _databaseServices.InsertAysnc(postobject);

                    //publish the new message to the further-topic
                    await _producerService.WriteMessage(_config["Kafka:further-topic"], JsonConvert.SerializeObject(newvalue));

                    transaction.Commit();
                    return postobject;
                }
                catch (ProduceException<Null, string>)
                {
                    throw;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public async Task<GroupModel> UpdateAsync(UpdateDTO request)
        {
            using (var transaction = _databaseServices._Context.Database.BeginTransaction())
            {
                //get the current record from the database if already present.
                var oldvalue = await _databaseServices.GetbyIdAsync(request.Id);
                var newvalue = _mapper.Map<UpdateDTO, GroupModel>(request, oldvalue);


                //comapre 2 objects
                bool issame = true;
                foreach (PropertyInfo prop in oldvalue.GetType().GetProperties())
                {
                    var newcolumnvalue = (newvalue.GetType().GetProperty(prop.Name).GetValue(newvalue, null));
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

                        return newvalue;
                    }
                    catch (ProduceException<Null, string>)
                    {
                        throw;
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                }
                return null;
            }
        }

        /// <summary>
        /// Adds new entitlements to the current list of entitlements.
        /// </summary>
        public async Task AddEntitlements(PatchDTO request)
        {
            var currentrecord = await _databaseServices.GetbyIdAsync(request.Id);

            //var obj = _mapper.Map<PatchDTO, GroupModel>(request, currentrecord);



        }

    }
}
