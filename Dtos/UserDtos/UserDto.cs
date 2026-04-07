using project_basic.Dtos.RoleDtos;

namespace project_basic.Dtos.UserDtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int? Age { get; set; }
    public string Address { get; set; } = string.Empty;
    public RoleDto Role { get; set; }
}