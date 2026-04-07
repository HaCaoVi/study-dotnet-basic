using project_basic.Dtos.UserDtos;

namespace project_basic.Dtos.CompanyDto;

public class CompanyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } =  string.Empty;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    public UserDto User { get; set; }
}