namespace BlogAPI.Helper;

using BlogAPI.DTOs;
using BlogAPI.Models;

public class FormatReturnUser
{
  public static GetUserDetail Map(User user)
  {
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