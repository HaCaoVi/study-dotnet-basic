using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_basic.Common;
using project_basic.Common.Responses;
using project_basic.Dtos.UserDtos;
using project_basic.Models;
using project_basic.Services.Interfaces;

namespace project_basic.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<UserDto>>>> GetUsers([FromQuery] QueryUserDto queryUserDto,CancellationToken ct)
    {
        var users = await _userService.GetAllUsersAsync(queryUserDto, ct);
        return Ok(ApiResponse<PagedResult<UserDto>>.Success(users, "Users retrieved successfully."));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetUser(Guid id, CancellationToken ct)
    {
        var user = await _userService.GetUserByIdAsync(id, ct);
        return Ok(ApiResponse<UserDto>.Success(user, "User retrieved successfully."));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser([FromBody] CreateUserDto createUserDto, CancellationToken ct)
    {
        var user = await _userService.CreateUserAsync(createUserDto, ct);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, ApiResponse<UserDto>.Success(user, "User created successfully.",201));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateUser(Guid id, [FromBody] UpdateUserDto updateUserDto, CancellationToken ct)
    {
        await _userService.UpdateUserAsync(id, updateUserDto, ct);
        return Ok(ApiResponse<object>.Success(null, "User updated successfully."));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteUser(Guid id, CancellationToken ct)
    {
        await _userService.DeleteUserAsync(id, ct);
        return Ok(ApiResponse<object>.Success(null, "User deleted successfully."));
    }
}