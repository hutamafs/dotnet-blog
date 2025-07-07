using BlogAPI.DTOs;
using BlogAPI.Models;
namespace BlogAPI.Services;

public interface IJwtService
{
  Task<LoginResponse?> LoginAndVerifyJwt(LoginRequest rq);
}
