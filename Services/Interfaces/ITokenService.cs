using project_basic.Dtos.UserDtos;
using project_basic.Models;

namespace project_basic.Services.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user,  int expiresIn);
    string HashToken(string token);
}