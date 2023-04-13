using AutoMapper;
using bookNookStarterCode.DataTransferObjects;
using bookNookStarterCode.Models;

namespace bookNookStarterCode.Managers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserForRegistrationDto, User>();
        }
    }
}
