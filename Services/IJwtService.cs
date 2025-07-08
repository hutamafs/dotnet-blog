using BlogAPI.DTOs;
namespace BlogAPI.Services;

public interface IJwtService
{
  Task<LoginResponse?> LoginAndVerifyJwt(LoginRequest rq);
}
