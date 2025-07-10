using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BlogAPI.DTOs;
using BlogAPI.Models;
using BlogAPI.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace BlogAPI.Services;

public class JwtService(IUserRepository repo, SymmetricSecurityKey jwtKey) : IJwtService
{
  private readonly IUserRepository _repo = repo;
  private readonly SymmetricSecurityKey _jwtKey = jwtKey;

  private string GenerateJwtToken(User user)
  {
    var tokenHandler = new JwtSecurityTokenHandler();
    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(
      [
      new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new Claim(ClaimTypes.Email, user.Email)
    ]),
      Expires = DateTime.UtcNow.AddHours(2),
      SigningCredentials = new SigningCredentials(
        _jwtKey,
        SecurityAlgorithms.HmacSha256Signature)
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    var tokenString = tokenHandler.WriteToken(token);
    return tokenString;
  }

  public async Task<LoginResponse?> LoginAndVerifyJwt(LoginRequest rq)
  {
    var foundUser = await _repo.GetByEmailAsync(rq.Email) ?? throw new HttpException("Unauthorized", 401, "email not found");
    ;
    var hasher = new PasswordHasher<User>();
    var verificationResult = hasher.VerifyHashedPassword(foundUser, foundUser.PasswordHash, rq.Password);
    if (verificationResult == PasswordVerificationResult.Success)
    {
      var token = GenerateJwtToken(foundUser);
      if (!string.IsNullOrEmpty(token)) return new LoginResponse
      {
        Email = rq.Email,
        Access_token = token
      };
    }
    throw new HttpException("Unauthorized", 401, "Invalid Password");


  }
}
