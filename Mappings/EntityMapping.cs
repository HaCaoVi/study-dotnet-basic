using AutoMapper;
using project_basic.Dtos.CompanyDto;
using project_basic.Dtos.RoleDtos;
using project_basic.Dtos.UserDtos;
using project_basic.Entities;

namespace project_basic.Mappings;

public class EntityMapping: Profile
{
        public EntityMapping()
        {
                CreateMap<User, UserDto>();
                CreateMap<Role, RoleDto>(); 
                CreateMap<Company, CompanyDto>();
        }

}