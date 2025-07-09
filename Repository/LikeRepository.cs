
using BlogAPI.Data;
using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Repository;

public class LikeRepository(AppDbContext context) : ILikeRepository
{
  private readonly AppDbContext _context = context;

  public async Task<bool> FindLike(Like like)
  {
    return await _context.Likes.AnyAsync(l => l.PostId == like.PostId && l.UserId == like.UserId);
  }
  public async Task LikePost(Like like)
  {
    _context.Likes.Add(like);
    await _context.SaveChangesAsync();
  }

  public async Task UnlikePost(Like like)
  {
    _context.Likes.Remove(like);
    await _context.SaveChangesAsync();
  }
}