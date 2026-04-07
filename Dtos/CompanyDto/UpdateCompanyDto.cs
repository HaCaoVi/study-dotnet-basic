namespace project_basic.Dtos.CompanyDto;

public class UpdateCompanyDto
{
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } =  string.Empty;
        public Guid UserId { get; set; }
}