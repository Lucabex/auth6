using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using auth6.Models;
using Microsoft.IdentityModel.Tokens;

namespace auth6.Services;

public class JwtServices
{
    private readonly IConfiguration _configuration;
    private readonly SymmetricSecurityKey _key;

    public JwtServices(IConfiguration configuration)
    {
        _configuration= configuration;
        var secretKey = _configuration["JwtSettings:SecretKey"];
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
    }
    public string GenerateToken(User user)
    {
        //Generate the claim

        var claim = new[]
        {
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new Claim(ClaimTypes.Name , (user.Name ?? "Unknwon")),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claim),
            Issuer = _configuration["JwtSetings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"],
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpirationInMinutes"])),
            SigningCredentials = new SigningCredentials(_key , SecurityAlgorithms.HmacSha256)  
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
}

