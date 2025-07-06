using BlogAPI.DTOs;
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

  async Task<GetAllDataDto<Post>> IPostRepository.GetAllPosts(PostQueryParamDto q)
  {
    IQueryable<Post> postQuery = _context.Posts.Include(p => p.Category).Include(p => p.User);

    if (!string.IsNullOrEmpty(q.Q))
    {
      postQuery = postQuery.Where(p =>
        (p.Title != null && p.Title.Contains(q.Q)) ||
        (p.Content != null && p.Content.Contains(q.Q)) ||
        (p.User != null && (
          (!string.IsNullOrEmpty(p.User.Firstname) && p.User.Firstname.Contains(q.Q)) ||
          (!string.IsNullOrEmpty(p.User.Lastname) && p.User.Lastname.Contains(q.Q))
        )) ||
        (p.Category != null && !string.IsNullOrEmpty(p.Category.Name) && p.Category.Name.Contains(q.Q))
      );
    }

    if (!string.IsNullOrEmpty(q.Title))
    {
      postQuery = postQuery.Where(p => p.Title.Contains(q.Title));
    }

    if (!string.IsNullOrEmpty(q.Content))
    {
      postQuery = postQuery.Where(p => p.Content.Contains(q.Content));
    }

    if (q.IsPublished != null)
    {
      postQuery = postQuery.Where(p => p.IsPublished == q.IsPublished);
    }

    if (q.UserId.HasValue)
    {
      postQuery = postQuery.Where(p => p.UserId == q.UserId);
    }

    if (q.CreatedAt.HasValue)
    {
      postQuery = postQuery.Where(p => p.CreatedAt == q.CreatedAt);
    }

    if (q.PublishedAt.HasValue)
    {
      postQuery = postQuery.Where(p => p.PublishedAt == q.PublishedAt);
    }

    if (q.UpdatedAt.HasValue)
    {
      postQuery = postQuery.Where(p => p.UpdatedAt == q.UpdatedAt);
    }

    var total = await postQuery.CountAsync();
    var totalPages = (int)Math.Ceiling((double)total / q.Take);

    postQuery = postQuery
    .Skip((q.PageNumber - 1) * q.Take)
    .Take(q.Take);

    var posts = await postQuery.ToListAsync();
    return new GetAllDataDto<Post>
    {
      Data = posts,
      Total = total,
      TotalPages = totalPages,
      PageNumber = q.PageNumber
    };
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