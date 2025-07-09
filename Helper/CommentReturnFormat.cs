namespace BlogAPI.Helper;

using BlogAPI.DTOs;
using BlogAPI.Models;

public class FormatReturnComment
{
  public static GetCommentDetail Map(Comment c)
  {
    return new GetCommentDetail
    {
      Id = c.Id,
      Text = c.Text,
      Author = $"{c.User?.Firstname} {c.User?.Lastname}".Trim(),
      CommentAt = c.CreatedAt,
      UpdatedAt = c.UpdatedAt,
      UserId = c.UserId
    };
  }
}