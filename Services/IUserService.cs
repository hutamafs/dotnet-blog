using BlogAPI.DTOs;
using BlogAPI.Models;
namespace BlogAPI.Services;

public interface IUserService
{
  Task<GetUserDetail?> GetUserDetail(int id);
  Task<User> CreateUser(CreateUpdateUserRequest rq);
}
