namespace project_basic.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = null!;
    public string Password { get; set; } =  null!;
    public string Name { get; set; } =  null!;
    public int? Age { get; set; }
    public string Address { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}