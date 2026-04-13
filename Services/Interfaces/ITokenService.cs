using System.Security.Claims;
using project_basic.Entities;

namespace project_basic.Services.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user,  int expiresIn);
    string HashToken(string token);
    ClaimsPrincipal ValidateRefreshToken(string token);
}