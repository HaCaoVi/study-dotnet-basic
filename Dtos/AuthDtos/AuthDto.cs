namespace project_basic.Dtos.AuthDtos;

public class AuthDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string Name { get; set; }
}