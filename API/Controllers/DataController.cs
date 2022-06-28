using Microsoft.AspNetCore.Mvc;
using Confluent.Kafka;
using Database.Models;
using API.DTOs;
using AutoMapper;

namespace API.Controllers
{
    [Route("resource")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly IAPIServices _APIServices; //API services
        private readonly ILogger<APIServices> _logger;
        private readonly IMapper _mapper;

        public DataController(IAPIServices APIServices, ILogger<APIServices> logger,IMapper mapper)
        {
            _APIServices = APIServices;
            _logger = logger;
            _mapper = mapper;
        }


        /// <summary>
        /// To GET all records from the database
        /// </summary>
        /// <returns> List of record</returns>
        [HttpGet]
        public async Task<ActionResult<List<GroupModel>>> GetList()
        {
            try
            {
                return await _APIServices.GetAllRecords();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>
        /// To GET current record from the database
        /// </summary>
        /// <param name="Id">Guid of the record</param>
        /// <returns></returns>
        [HttpGet("{Id}")]
        public async Task<ActionResult<GroupModel>> Get(Guid? Id)
        {
            //_logger.LogInformation("API:  GET method invoked");
            try
            {
                var currentrecord = await _APIServices.GetSingleRowById(Id);
                if (currentrecord == null) return NotFound("No such Id found!!!");

                return Ok(currentrecord);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        /// <summary>
        /// To INSERT a new record
        /// </summary>
        /// <param name="mymodel">A single Record</param>

        [HttpPost]
        public async Task<ActionResult<GroupModel>> Post(AddDataDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var inserted=await _APIServices.InsertAsync(request);
                return Ok(inserted);
            }
            catch (ProduceException<Null, string>)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal message Bus exception" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// To UPDATE a whole record.
        /// </summary>
        /// <param name="request">A single Record</param>
        [HttpPut]
        public async Task<ActionResult<GroupModel>> Put(UpdateDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var record = await _APIServices.GetSingleRowById(request.Id);
                if (record == null) return BadRequest("No such record exists to update");

                var updated=await _APIServices.UpdateAsync(request);
                if (updated == null) return BadRequest("Duplicate data entered");

                return Ok(updated);
            }
            catch (ProduceException<Null, string>)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal message Bus exception" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }


        /// <summary>
        /// To ADD entitlements for a speicific ID.
        /// </summary>
        /// <returns> The updated record</returns>
        [HttpPatch]
        public async Task<ActionResult<GroupModel>> Patch(PatchDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var record = await _APIServices.GetSingleRowById(request.Id);
            if (record == null) return BadRequest("No such record exists to update");

            var updated=await _APIServices.AddEntitlements(request);
            if (updated == null) return BadRequest("Duplicate data entered");
            return Ok(updated);
        }
    }
}
