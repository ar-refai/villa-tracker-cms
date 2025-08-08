using AutoMapper;
using VillaManager.Data.EntityModel;
using VillaManager.Data.Models;
using VillaManager.Domain.DTOs;
using VillaManager.Domain.DTOs.UsersDTO;
using VillaManager.Domain.DTOs.VillaDTO;

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
            // Map Villa to VillaDto
            CreateMap<Villa, VillaDto>()
                .ForMember(dest => dest.Files, opt => opt.MapFrom(src => src.Files));
            
            CreateMap<VillaFile, VillaFileDto>();
            CreateMap<VillaFileDto, VillaFile>();

            CreateMap<VillaDto, Villa>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore Id on create
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow)) // Set CreatedAt to current time on create
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false)); // Default IsDeleted to false on create
                                                                                      // Map VillaCreateDto to Villa
            // This mapping is used when creating a new villa
            CreateMap<VillaCreateDto, Villa>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore Id on create
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow)) // Set CreatedAt to current time on create
                .ForMember(dest => dest.Files, opt=> opt.Ignore());  // added this newly to ignore Files during mapping

            // This mapping is used when updating an existing villa
            CreateMap<VillaCreateDto, Villa>();
            CreateMap<VillaUpdateDto, Villa>()
                .ForMember(dest => dest.Files, opt => opt.Ignore()); // We'll handle files manually

        }
    }
}
