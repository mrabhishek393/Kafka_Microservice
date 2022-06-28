using API.DTOs;
using AutoMapper;
using Confluent.Kafka;
using Database;
using Database.Models;
using Newtonsoft.Json;
using Producer;
using System.Reflection;
using System.Transactions;

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
        private readonly IMapper _mapper;

        public APIServices(IConfiguration config,
            IDatabaseServices databaseServices,
            IProducerService producerService,
            IMapper mapper
            )
        {
            _config = config;
            _databaseServices = databaseServices;
            _producerService = producerService;
            _mapper = mapper;
        }

        public async Task<List<GroupModel>> GetAllRecords()
        {
            try
            {
                return await _databaseServices.GetAllAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// get the current record from the database using ID if already present other wise null
        /// </summary>
        public async Task<GroupModel> GetSingleRowById(Guid? id)
        {
            try
            {
                return await _databaseServices.GetbyIdAsync(id);
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Inserts the new record in the database if not already present otherwise updates the record.
        /// And publishes it to further-topic.
        /// </summary>
        public async Task<GroupModel> InsertAsync(AddDataDTO newvalue)
        {
            try
            {
                using (var transactionScope = new TransactionScope())
                {

                    GroupModel postobject = _mapper.Map<GroupModel>(newvalue);

                    //insert the new data
                    await _databaseServices.InsertAysnc(postobject);

                    //publish the new message to the further-topic
                    await _producerService.WriteMessage(_config["Kafka:further-topic"], JsonConvert.SerializeObject(postobject));

                    transactionScope.Complete();
                    return postobject;

                }
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

        public async Task<GroupModel> UpdateAsync(UpdateDTO request)
        {

            try
            {
                using (var transactionScope = new TransactionScope())
                {

                    //get the current record from the database if already present.
                    var oldvalue = await _databaseServices.GetbyIdAsync(request.Id);
                    var newvalue = _mapper.Map<UpdateDTO, GroupModel>(request);


                    newvalue.CreatedAt = oldvalue.CreatedAt;
                    newvalue.Entitlements = newvalue.Entitlements.Distinct().ToList();

                    //compare 2 objects
                    bool issame = true;

                    if (newvalue.Name != oldvalue.Name)
                    {
                        issame = false;
                    }
                    var new_entitlements_except_old = newvalue.Entitlements.Except(oldvalue.Entitlements);

                    //Console.WriteLine(new_entitlements_except_old.Count());
                    if (new_entitlements_except_old.Count() != 0)
                    {
                        issame = false;
                    }

                    //new data published on kafka is redundant information ==> issame=true
                    // if issame=false, updation must be done.
                    if (!issame)
                    {

                        //Update the data in the database
                        await _databaseServices.UpdateAsync(oldvalue, newvalue);

                        // ==> publish the updated record in the further-topic
                        await _producerService.WriteMessage(_config["Kafka:further-topic"], JsonConvert.SerializeObject(newvalue));
                        transactionScope.Complete();

                        return newvalue;
                    }
                    return null;
                }
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

        /// <summary>
        /// Adds new entitlements to the current list of entitlements.
        /// </summary>
        public async Task<GroupModel> AddEntitlements(PatchDTO request)
        {
            try
            {
                using (var transactionScope = new TransactionScope())
                {
                    var oldvalue = await _databaseServices.GetbyIdAsync(request.Id);

                    var entitlements_from_db = oldvalue.Entitlements;
                    var entitlements_from_request = request.Entitlements.Distinct();
                    var new_entitlements = entitlements_from_request.Except(entitlements_from_db);

                    if (new_entitlements.Count() == 0) return null;

                    request.Entitlements = entitlements_from_db.Concat(new_entitlements).ToList();

                    var newvalue = _mapper.Map<PatchDTO, GroupModel>(request);

                    newvalue.Name = oldvalue.Name;
                    newvalue.CreatedAt = oldvalue.CreatedAt;

                    await _databaseServices.UpdateAsync(oldvalue, newvalue);

                    await _producerService.WriteMessage(_config["Kafka:further-topic"], JsonConvert.SerializeObject(newvalue));

                    transactionScope.Complete();
                    return newvalue;
                }
                
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
}
