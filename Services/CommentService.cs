using System.Security.Claims;
using BlogAPI.DTOs;
using BlogAPI.Models;
using BlogAPI.Repository;

namespace BlogAPI.Services;

public class CommentService(ICommentRepository commentRepo, IUserRepository userRepo) : ICommentService
{
  private readonly ICommentRepository _commentRepo = commentRepo;
  private readonly IUserRepository _userRepo = userRepo;

  public async Task<GetCommentDetail> PostComment(CreateUpdateCommentRequest rq)
  {
    if (!await _commentRepo.IsPostIdExist(rq.PostId))
      throw new HttpException("Not Found", 404, "Post does not exist");

    var comment = new Comment
    {
      Text = rq.Text,
      UserId = rq.UserId,
      PostId = rq.PostId,
    };
    var createdComment = await _commentRepo.PostCommentAsync(comment);
    var user = await _userRepo.GetByIdAsync(rq.UserId);

    return new GetCommentDetail
    {
      Id = createdComment.Id,
      Author = user?.Username!,
      Text = createdComment.Text,
      CommentAt = createdComment.CreatedAt,
      UpdatedAt = createdComment.UpdatedAt,
    };
  }

  public async Task<GetCommentDetail?> GetCommentById(int id)
  {
    var comment = await _commentRepo.GetCommentById(id);
    if (comment == null) return null;
    var user = await _userRepo.GetByIdAsync(comment.UserId);
    return new GetCommentDetail
    {
      Id = comment.Id,
      Author = user?.Username!,
      Text = comment.Text,
      CommentAt = comment.CreatedAt,
      UpdatedAt = comment.UpdatedAt,
    };
  }
}
