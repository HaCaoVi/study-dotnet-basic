namespace project_basic.Dtos.AuthDtos;

public class AuthDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}