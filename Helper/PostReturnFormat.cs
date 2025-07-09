namespace BlogAPI.Helper;

using BlogAPI.DTOs;
using BlogAPI.Models;

public class FormatReturnPost
{
  public static GetPostDetail Map(Post post, User user)
  {
    return new GetPostDetail
    {
      Id = post.Id,
      Title = post.Title,
      Content = post.Content,
      Slug = post.Slug,
      UserId = post.UserId,
      Author = $"{user?.Firstname} {user?.Lastname}".Trim(),
      CategoryName = post.Category?.Name,
      IsPublished = post.IsPublished,
      CreatedAt = post.CreatedAt,
      UpdatedAt = post.UpdatedAt,
      PublishedAt = post.PublishedAt,
      Comments = [.. post.Comments
      .OrderByDescending(p => p.UpdatedAt)
      .Take(20)
      .Select(FormatReturnComment.Map)],
      Likes = [ ..post.LikedByUser
      .OrderByDescending(l => l.UpdatedAt)
      .Select(l => new LikeByUserResponse {
        LikedAt = l.CreatedAt,
        Username = l.User?.Username!,
      })
      ]
    };
  }
}