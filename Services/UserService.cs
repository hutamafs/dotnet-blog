using BlogAPI.DTOs;
using BlogAPI.Helper;
using BlogAPI.Models;
using BlogAPI.Repository;
using Microsoft.AspNetCore.Identity;

namespace BlogAPI.Services;

public class UserService(IUserRepository repo) : IUserService
{
  private readonly IUserRepository _userRepo = repo;

  public async Task<GetUserDetail> CreateUser(CreateUserRequest rq)
  {
    if (await _userRepo.IsEmailTakenAsync(rq.Email))
      throw new HttpException("Conflict", 409, "Email already in use");

    if (await _userRepo.IsUsernameTakenAsync(rq.Username))
      throw new HttpException("Conflict", 409, "Username already taken");
    var hasher = new PasswordHasher<User>();

    var user = new User(rq.Username, rq.Email, hasher.HashPassword(new User(), rq.Password), rq.Firstname, rq.Lastname, rq.Bio);

    await _userRepo.AddAsync(user);
    return FormatReturnUser.Map(user);
  }

  public async Task<GetUserDetail?> GetUserDetail(int id)
  {
    User? user = await _userRepo.GetByIdAsync(id);
    if (user == null) return null;
    return new GetUserDetail
    {
      Id = user.Id,
      Email = user.Email,
      Firstname = user.Firstname,
      Lastname = user.Lastname,
      Bio = user.Bio,
      Posts = [.. user.Posts
      .OrderByDescending(p => p.UpdatedAt)
      .Take(10)
      .Select(p => FormatReturnPost.Map(p, user))],
    };
  }

  public async Task<GetUserDetail?> UpdateUser(int id, UpdateUserProfileRequest rq)
  {
    User? user = await _userRepo.GetByIdAsync(id);
    if (user == null) return null;
    user.Bio = rq.Bio;
    user.Firstname = rq.Firstname;
    user.Lastname = rq.Lastname;
    user.UpdatedAt = DateTime.Now;

    await _userRepo.SaveChangesAsync();
    return new GetUserDetail
    {
      Id = user.Id,
      Email = user.Email,
      Firstname = user.Firstname,
      Lastname = user.Lastname,
      Bio = user.Bio,
      Posts = [.. user.Posts
      .OrderByDescending(p => p.UpdatedAt)
      .Take(10)
      .Select(p => FormatReturnPost.Map(p, user))],
    };
  }
}
