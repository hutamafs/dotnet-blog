using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Repository;

public class PostRepository(AppDbContext context) : IPostRepository
{
  private readonly AppDbContext _context = context;

  async Task<bool> IPostRepository.IsUserIdExist(int id)
  {
    return await _context.Users.AnyAsync(u => u.Id == id);
  }
  async Task<bool> IPostRepository.IsCategoryIdExist(int id)
  {
    return await _context.Categories.AnyAsync(u => u.Id == id);
  }
  async Task IPostRepository.CreatePostAsync(Post post)
  {
    _context.Posts.Add(post);
    await _context.SaveChangesAsync();
  }

  async Task<bool> IPostRepository.DeletePost(Post post)
  {
    _context.Posts.Remove(post);
    return await _context.SaveChangesAsync() > 0;
  }

  async Task<IEnumerable<Post>> IPostRepository.GetAllPosts()
  {
    return await _context.Posts.Include(b => b.Category).ToListAsync();

  }

  async Task<Post?> IPostRepository.GetByIdAsync(int id)
  {
    return await _context.Posts.Include(b => b.Category).FirstOrDefaultAsync(p => p.Id == id);
  }

  public async Task SaveChangesAsync()
  {
    await _context.SaveChangesAsync();
  }

  public Task<Post> UpdatePost(int id, Post post)
  {
    throw new NotImplementedException();
  }
}