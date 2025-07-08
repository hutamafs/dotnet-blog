namespace BlogAPI.Repository;

using BlogAPI.DTOs;
using BlogAPI.Models;

public interface ICommentRepository
{
  Task<bool> IsPostIdExist(int id);
  Task<Comment?> GetCommentById(int id);
  Task<Comment> PostCommentAsync(Comment comment);
  Task SaveChangesAsync();
  Task<Post?> UpdateCommentAsync(int id, Comment comment);
  Task<bool> DeleteComment(int id);
}