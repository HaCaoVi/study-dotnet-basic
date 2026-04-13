namespace project_basic.Common.Configuration;

public class JwtSettings
{
    public const string SectionName = "JWT";

    public string SigningKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; } = 30;
    public int RefreshTokenExpirationMinutes { get; set; } = 43200; // 30 days
}
