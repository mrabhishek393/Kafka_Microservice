using API.DTOs;
using AutoMapper;
using Database.Models;

namespace API.Models
{
    public class MappingProfiles: Profile
    {
        public MappingProfiles()
        {
            CreateMap<AddDataDTO, GroupModel>().AfterMap((src, dest) =>
            {
                dest.Id = Guid.NewGuid();
                dest.CreatedAt = DateTime.Now;
            });

            CreateMap<UpdateDTO, GroupModel>();


            CreateMap<PatchDTO, GroupModel>();
        }
    }
}
