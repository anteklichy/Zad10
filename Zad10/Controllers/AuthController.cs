using Microsoft.AspNetCore.Mvc;
using Zad10.Dtos;
using Zad10.Services;

namespace Zad10.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;

    public AuthController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
    {
        await _userService.RegisterUser(userRegisterDto);
        return Ok(new { message = "Registration successful" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto userLoginDto)
    {
        var tokens = await _userService.Authenticate(userLoginDto);
        return Ok(tokens);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
    {
        var tokens = await _userService.RefreshToken(refreshTokenDto.RefreshToken);
        return Ok(tokens);
    }
}