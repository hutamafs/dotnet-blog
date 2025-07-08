namespace BlogAPI.Repository;

using System.Threading.Tasks;
using BlogAPI.Data;
using BlogAPI.DTOs;
using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;

public class CommentRepository(AppDbContext context) : ICommentRepository
{

  private readonly AppDbContext _context = context;

  public Task<bool> DeleteComment(int id)
  {
    throw new NotImplementedException();
  }

  public Task<GetAllDataDto<Post>> GetCommentsForPost(int id)
  {
    throw new NotImplementedException();
  }

  public async Task<Comment?> GetCommentById(int id)
  {
    return await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
  }

  public async Task<bool> IsPostIdExist(int id)
  {
    return await _context.Posts.AnyAsync(p => p.Id == id);
  }

  public async Task<Comment> PostCommentAsync(Comment comment)
  {
    _context.Comments.Add(comment);
    await _context.SaveChangesAsync();
    return comment;
  }

  public Task SaveChangesAsync()
  {
    throw new NotImplementedException();
  }

  public Task<Post?> UpdateCommentAsync(int id, Comment comment)
  {
    throw new NotImplementedException();
  }
}