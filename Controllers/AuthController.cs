using Microsoft.AspNetCore.Mvc;
using TaskManager.API.DTOs;
using TaskManager.API.Services;

namespace TaskManager.API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AuthController : ControllerBase
  {
    private readonly AuthService _authService;
    private readonly IConfiguration _configuration;

    public AuthController(AuthService authService, IConfiguration configuration)
    {
      _authService = authService;
      _configuration = configuration;
    }

    // In AuthController.cs (just for testing)
    [HttpGet("test-config")]
    public IActionResult TestConfig()
    {
      var key = _configuration["Jwt:Key"];
      return Ok(new
      {
        keyExists = !string.IsNullOrEmpty(key),
        keyLength = key?.Length ?? 0,
      });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
      // Validate input
      if (string.IsNullOrWhiteSpace(registerDto.Username) ||
          string.IsNullOrWhiteSpace(registerDto.Email) ||
          string.IsNullOrWhiteSpace(registerDto.Password))
      {
        return BadRequest(new { message = "All fields are required" });
      }

      // Call service
      var result = await _authService.Register(registerDto);

      if (result == null)
      {
        return BadRequest(new { message = "Username or email already exists" });
      }

      return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
      // Validate input
      if (string.IsNullOrWhiteSpace(loginDto.Email) ||
          string.IsNullOrWhiteSpace(loginDto.Password))
      {
        return BadRequest(new { message = "Email and password are required" });
      }

      // Call service
      var result = await _authService.Login(loginDto);

      if (result == null)
      {
        return Unauthorized(new { message = "Invalid email or password" });
      }

      return Ok(result);
    }
  }
}