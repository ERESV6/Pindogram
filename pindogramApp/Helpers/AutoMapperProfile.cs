using AutoMapper;
using pindogramApp.Dtos;
using pindogramApp.Entities;


namespace pindogramApp.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<Meme, MemeDto>();
            CreateMap<MemeDto, Meme>();
        }
    }
}
