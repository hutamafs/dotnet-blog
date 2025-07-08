using BlogAPI.DTOs;
using BlogAPI.Models;
namespace BlogAPI.Services;

public interface IUserService
{
  Task<GetUserDetail?> GetUserDetail(int id);
  Task<GetUserDetail> CreateUser(CreateUserRequest rq);
  Task<GetUserDetail?> UpdateUser(int id, UpdateUserProfileRequest rq);
}
