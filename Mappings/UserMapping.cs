using AutoMapper;
using project_basic.Dtos.UserDtos;
using project_basic.Models;

namespace project_basic.Mappings;

public class UserMapping: Profile
{
    public UserMapping()
    {
        CreateMap<User, UserDto>();
    }
}