using AutoMapper;
using extraAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace extraAPI.Controllers
{

    [Route("resource")]
    [ApiController]
    public class TestController : ControllerBase
    {

        private readonly TestdbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<TestController> _logger;

        public TestController(TestdbContext context,IMapper mapper,ILogger<TestController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger=logger;
        }
        [HttpGet]

        public async Task<ActionResult<List<GroupInfo>>> GetList()
        {
            //if(EventLo)
            _logger.LogInformation("In the get list method");
            return await _context.GroupInfos.ToListAsync();
        }
        [HttpGet("{Id}")]
        public async Task<ActionResult<GroupInfo>> Get(Guid Id)
        {
            var v=await _context.GroupInfos.FindAsync(Id);
            if (v != null) return Ok(v);
            return NotFound("No such record exists");
        }

        [HttpPost]
        public async Task<ActionResult<GroupInfo>> Post(AddDataDTO request)
        {
            if (request.Name == null) return BadRequest("Name not provided");

            //var l=new List<Guid>();
            //GroupInfo postobject = new GroupInfo
            //{
            //    Id = Guid.NewGuid(),
            //    Name = request.Name,
            //    Entitlements = ((request.Entitlements == null) ? l : request.Entitlements),
            //    CreatedAt = DateTime.Now
            //};
            GroupInfo postobject= _mapper.Map<GroupInfo>(request);

            //var currentrecord = await _context.GroupInfos.FindAsync(postobject.Id);
            //if (currentrecord == null)
            //{
            //    await _context.GroupInfos.AddAsync(postobject);
            //    await _context.SaveChangesAsync();
            //    return Ok(postobject);
            //}
            //return BadRequest("Record already exists");
            return Ok(postobject);
        }

        [HttpPut]
        public async Task<ActionResult<GroupInfo>> Patch(UpdateDTO request)
        {
            var currentrecord = await _context.GroupInfos.FindAsync(request.Id);
            if (currentrecord != null)
            {
                var obj=_mapper.Map<UpdateDTO,GroupInfo>(request, currentrecord);
                return Ok(obj);
                //var t = currentrecord.Entitlements;
                //if (Duplicate(t, entId)) return BadRequest("Already exists in list");
                //currentrecord.Entitlements.Add(entId);
                //currentrecord.CreatedAt = DateTime.Now;
                //_context.GroupInfos.Update(currentrecord);
                //await _context.SaveChangesAsync();
                //currentrecord = await _context.GroupInfos.FindAsync(id);
                //return Ok(currentrecord);
            }
            return BadRequest("Something went wrong");
        }

        [HttpPatch]

        public async Task<ActionResult<GroupInfo>> Patch(PatchDTO request)
        {
            var currentrecord = await _context.GroupInfos.FindAsync(request.Id);
            if (currentrecord != null)
            {
                var obj = _mapper.Map<PatchDTO, GroupInfo>(request, currentrecord);
                return Ok(obj);
            }
            return BadRequest("Something went wrong");
        }

        [HttpPost("Duplicate check")]
        public async Task<ActionResult<GroupInfo>> Get(UpdateDTO request)
        {
            var x = new UpdateDTO
            {
                Id = new Guid("07dd5952-c353-4bf9-815f-9c9de0c197fa"),
                Name = "Wohooo",
                Entitlements = new List<Guid>() { new Guid("1a347707-557f-4cf8-be96-c9ed851bb51a") }
            };
            var currentrecord = await _context.GroupInfos.FindAsync(new Guid("07dd5952-c353-4bf9-815f-9c9de0c197fa"));

            var c = x.Entitlements;
            var t = currentrecord.Entitlements;

            for (int i = 0; i < c.Count(); i++) Console.Write(c[i]);
            Console.WriteLine();
            for (int i = 0; i < t.Count(); i++) Console.Write(t[i]);
            Console.WriteLine();
            var new_entitlements_except_old=c.Except(t).ToList();
            Console.WriteLine(new_entitlements_except_old.Count());

            return Ok(x);
        }
        private bool Duplicate(List<Guid> ent,Guid nent)
        {
            if (ent.Contains(nent)) return true;
            return false;
        }
    }
}
