using Microsoft.AspNetCore.Mvc;
using project_basic.Common;
using project_basic.Dtos.UserDtos;

namespace project_basic.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    
    [HttpGet()]
    public async Task<IActionResult> GetUsers()
    {
        return Ok(ResponseFactory.Response("TEST API"));
    }
    
    [HttpPost()]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        return Ok(ResponseFactory.Response("TEST API"));
    }
}