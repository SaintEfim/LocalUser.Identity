using AutoMapper;
using Users.Domain.Entity;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Users.Api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegistrationRequest, LocalUser>();
        }
    }
}
