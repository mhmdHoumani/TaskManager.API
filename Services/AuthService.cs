using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManager.API.Data;
using TaskManager.API.DTOs;
using TaskManager.API.Models;

namespace TaskManager.API.Services
{
  public class AuthService
  {
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
      _context = context;
      _configuration = configuration;
    }

    public async Task<AuthResponseDto?> Register(RegisterDto registerDto)
    {
      // Check if user already exists
      if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
      {
        return null; // Email already exists
      }

      if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
      {
        return null; // Username already exists
      }

      // Hash the password
      string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

      // Create new user
      var user = new User
      {
        Username = registerDto.Username,
        Email = registerDto.Email,
        PasswordHash = passwordHash,
        Role = "User"
      };

      _context.Users.Add(user);
      await _context.SaveChangesAsync();

      // Generate JWT token
      var token = GenerateJwtToken(user);

      return new AuthResponseDto
      {
        Token = token,
        Username = user.Username,
        Email = user.Email,
        Role = user.Role
      };
    }

    public async Task<AuthResponseDto?> Login(LoginDto loginDto)
    {
      // Find user by email
      var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

      if (user == null)
      {
        return null; // User not found
      }

      // Verify password
      bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);

      if (!isPasswordValid)
      {
        return null; // Invalid password
      }

      // Generate JWT token
      var token = GenerateJwtToken(user);

      return new AuthResponseDto
      {
        Token = token,
        Username = user.Username,
        Email = user.Email,
        Role = user.Role
      };
    }

    private string GenerateJwtToken(User user)
    {
      var jwtKey = _configuration["Jwt:Key"];
      var jwtIssuer = _configuration["Jwt:Issuer"];
      var jwtAudience = _configuration["Jwt:Audience"];
      var jwtExpiryInMinutes = int.Parse(_configuration["Jwt:ExpiryInMinutes"]!);

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!));
      var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var claims = new[]
      {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

      var token = new JwtSecurityToken(
          issuer: jwtIssuer,
          audience: jwtAudience,
          claims: claims,
          expires: DateTime.UtcNow.AddMinutes(jwtExpiryInMinutes),
          signingCredentials: credentials
      );

      return new JwtSecurityTokenHandler().WriteToken(token);
    }
  }
}