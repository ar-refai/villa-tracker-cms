using AutoMapper;
using VillaManager.Data.EntityModel;
using VillaManager.Domain.DTOs;
using VillaManager.Domain.DTOs.UsersDTO;

namespace VillaManager.API.Mapping
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {


            // Map User to ShowUserDto (to expose user details to the API)
            CreateMap<User, ShowUserDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Name));

            // Map ShowUserDto to User (for updates, if necessary)
            CreateMap<ShowUserDto, User>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName));


        }
    }
}
