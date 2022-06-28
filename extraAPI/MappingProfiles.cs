using extraAPI.Models;
using AutoMapper;

namespace extraAPI
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<AddDataDTO, GroupInfo>().AfterMap((src, dest) =>
            {
                dest.Id = Guid.NewGuid();
                dest.CreatedAt = DateTime.Now;
            });

            CreateMap<UpdateDTO,GroupInfo>();


            CreateMap<PatchDTO, GroupInfo>().ForMember(dest => dest.Entitlements, ct =>
            {
                ct.UseDestinationValue();
                ct.Ignore();
            })
                .AfterMap((src, dest) => dest.Entitlements.AddRange(src.Entitlements));
        }
    }
}
