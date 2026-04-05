using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_basic.Common;
using project_basic.Dtos.AuthDtos;
using project_basic.Services.Interfaces;

namespace project_basic.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController: ControllerBase
{
    private readonly IAuthService _authService;
    
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<AuthDto>>> Login(LoginDto loginDto, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(loginDto, ct);
        return Ok(ApiResponse<AuthDto>.Success(result, "Login successfully"));
    }
}