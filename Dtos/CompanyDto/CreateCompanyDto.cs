namespace project_basic.Dtos.CompanyDto;

public class CreateCompanyDto
{
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public Guid UserId { get; set; }
}