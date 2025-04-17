using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BlinkIt.Repository.Interfaces;
using BlinkIt.Repository.Models;
using BlinkIt.Service.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace BlinkIt.Service.Implementation;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IConfiguration _configuration;
    public AuthService(IAuthRepository authRepository, IConfiguration config)
    {
        _authRepository = authRepository;
        _configuration = config;
    }
    public async Task<(bool Success, string Message, string Token)> CreateNewUser(string mobileNumber, string password)
    {
        var existingUser = await _authRepository.GetUserByMobileNumberAsync(mobileNumber);
        if (existingUser != null)
        {
            return (false, "Mobile number already exists. Cannot create new User.", null);
        }

        var newUser = new User
        {
            MobileNumber = mobileNumber,
            Password = password
        };

        await _authRepository.CreateUserAsync(newUser);

        var token = GenerateToken(mobileNumber);

        return (true, "New user created and login successful!", token);
    }
    
    private string GenerateToken(string mobileNumber)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
           
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]); 

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, mobileNumber) 
            }),
            Expires = DateTime.UtcNow.AddHours(1),
                
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}