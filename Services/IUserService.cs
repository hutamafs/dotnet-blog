using BlogAPI.DTOs;
using BlogAPI.Models;
namespace BlogAPI.Services;

public interface IUserService
{
  Task<GetUserDetail?> GetUserDetail(int id);
  Task<User> CreateUser(CreateUserRequest rq);
  Task<GetUserDetail?> UpdateUser(int id, UpdateUserProfileRequest rq);
}
