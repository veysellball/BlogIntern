using AutoMapper;
using BlogIntern.Dtos;
using BlogIntern.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // UserCreateDto → User mapping
        CreateMap<UserCreateDto, User>();
    }
}
