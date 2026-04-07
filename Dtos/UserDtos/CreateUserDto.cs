namespace project_basic.Dtos.UserDtos;

public class CreateUserDto
{
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int? Age { get; set; }
    public string Address { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
}