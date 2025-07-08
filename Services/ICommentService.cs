using BlogAPI.DTOs;
namespace BlogAPI.Services;

public interface ICommentService
{
  Task<GetCommentDetail> PostComment(CreateUpdateCommentRequest rq);
  Task<GetCommentDetail?> GetCommentById(int id);
  Task<GetCommentDetail?> UpdateCommentById(int id, CreateUpdateCommentRequest rq);
}
