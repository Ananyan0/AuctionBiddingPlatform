using AuctionBiddingPlatform.Core.DTOs.Auth;
using AuctionBiddingPlatform.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace AuctionBiddingPlatform.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        await _authService.RegisterAsync(dto.UserName, dto.Email, dto.Password);
        return Ok(new { Message = "User registered successfully." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var token = await _authService.LoginAsync(dto.UserName, dto.Password);
        return Ok(new LoginResponseDto { Token = token });
    }
}
