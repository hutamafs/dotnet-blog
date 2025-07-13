using BlogAPI.DTOs;
using BlogAPI.Helper;
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

    return FormatReturnComment.Map(createdComment);
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
      UserId = comment.UserId,
    };
  }

  public async Task<GetCommentDetail?> UpdateCommentById(int id, CreateUpdateCommentRequest rq)
  {
    try
    {
      Comment comment = new()
      {
        Id = id,
        Text = rq.Text,
        UserId = rq.UserId,
        PostId = rq.PostId
      };
      var result = await _commentRepo.UpdateCommentAsync(id, comment);
      if (result == null) return null;
      var user = await _userRepo.GetByIdAsync(comment.UserId);
      if (user == null) return null;
      return FormatReturnComment.Map(comment);
    }
    catch (System.Exception)
    {

      throw;
    }
  }

  public async Task DeleteCommentById(int id)
  {
    try
    {
      var foundComment = await _commentRepo.GetCommentById(id) ?? throw new HttpException("not found", 404, "comment not found");
      foundComment.IsDeleted = true;
      foundComment.DeletedAt = DateTime.Now;
      await _commentRepo.SaveChangesAsync();
    }
    catch (System.Exception)
    {

      throw;
    }
  }
}
