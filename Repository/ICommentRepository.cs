namespace BlogAPI.Repository;

using BlogAPI.Models;

public interface ICommentRepository
{
  Task<bool> IsPostIdExist(int id);
  Task<Comment?> GetCommentById(int id);
  Task<Comment> PostCommentAsync(Comment comment);
  Task SaveChangesAsync();
  Task<Comment?> UpdateCommentAsync(int id, Comment comment);
  Task DeleteComment(int id);
}