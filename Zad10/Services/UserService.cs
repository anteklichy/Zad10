using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Zad10.Dtos;
using Zad10.Models;
using Zad10.Repositories;

namespace Zad10.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public UserService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task RegisterUser(UserRegisterDto userRegisterDto)
    {
        var user = new User
        {
            Login = userRegisterDto.Login,
            Password = BCrypt.Net.BCrypt.HashPassword(userRegisterDto.Password)
        };

        await _userRepository.AddUser(user);
    }

    public async Task<AuthenticationResponseDto> Authenticate(UserLoginDto userLoginDto)
    {
        var user = await _userRepository.GetUserByLogin(userLoginDto.Login);
        if (!BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.Password))
            throw new UnauthorizedAccessException("Invalid credentials");

        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();

        await _userRepository.SaveRefreshToken(user.Id, refreshToken);

        return new AuthenticationResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthenticationResponseDto> RefreshToken(string refreshToken)
    {
        var user = await _userRepository.GetUserByRefreshToken(refreshToken);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        var accessToken = GenerateAccessToken(user);
        var newRefreshToken = GenerateRefreshToken();

        await _userRepository.SaveRefreshToken(user.Id, newRefreshToken);

        return new AuthenticationResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken
        };
    }

    private string GenerateAccessToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Login)
            }),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString();
    }
}

public interface IUserService
{
    Task RegisterUser(UserRegisterDto userRegisterDto);
    Task<AuthenticationResponseDto> Authenticate(UserLoginDto userLoginDto);
    Task<AuthenticationResponseDto> RefreshToken(string refreshToken);
}
