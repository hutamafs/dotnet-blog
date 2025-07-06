using BlogAPI.DTOs;
using BlogAPI.Models;
using BlogAPI.Repository;
using Microsoft.AspNetCore.Identity;

namespace BlogAPI.Services;

public class UserService(IUserRepository repo) : IUserService
{
  private readonly IUserRepository _userRepo = repo;

  public async Task<User> CreateUser(CreateUpdateUserRequest rq)
  {
    if (await _userRepo.IsEmailTakenAsync(rq.Email))
      throw new HttpException("Conflict", 409, "Email already in use");

    if (await _userRepo.IsUsernameTakenAsync(rq.Username))
      throw new HttpException("Conflict", 409, "Username already taken");
    var hasher = new PasswordHasher<object>();

    var user = new User(rq.Username, rq.Email, hasher.HashPassword(new object(), rq.Password), rq.Firstname, rq.Lastname, rq.Bio);

    await _userRepo.AddAsync(user);
    return user;
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
      .OrderByDescending(p => p.CreatedAt)
      .Take(10)
      .Select(p => new GetPostDetail
      {
        Id = p.Id,
        Title = p.Title,
        Content = p.Content,
        Slug = p.Slug,
        IsPublished = p.IsPublished,
        CreatedAt = p.CreatedAt,
        UserId = user.Id,
        PublishedAt = p.PublishedAt ?? DateTime.MinValue,
        UpdatedAt = p.UpdatedAt,
      })],
    };
  }
}
