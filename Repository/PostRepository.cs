using Microsoft.EntityFrameworkCore;
using BlogAPI.DTOs;
using BlogAPI.Models;
using BlogAPI.Data;

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

  async Task<GetAllDataDto<Post>> IPostRepository.GetAllPosts(PostQueryParamDto query)
  {
    IQueryable<Post> postQuery = _context.Posts
    .Include(p => p.Category)
    .Include(p => p.User)
    .Include(p => p.Comments);

    if (!string.IsNullOrEmpty(query.Q))
    {
      postQuery = postQuery.Where(p =>
        (p.Title != null && p.Title.Contains(query.Q)) ||
        (p.Content != null && p.Content.Contains(query.Q)) ||
        (p.User != null && (
          (!string.IsNullOrEmpty(p.User.Firstname) && p.User.Firstname.Contains(query.Q)) ||
          (!string.IsNullOrEmpty(p.User.Lastname) && p.User.Lastname.Contains(query.Q))
        )) ||
        (p.Category != null && !string.IsNullOrEmpty(p.Category.Name) && p.Category.Name.Contains(query.Q))
      );
    }

    if (!string.IsNullOrEmpty(query.Title))
    {
      postQuery = postQuery.Where(p => p.Title.Contains(query.Title));
    }

    if (!string.IsNullOrEmpty(query.Content))
    {
      postQuery = postQuery.Where(p => p.Content.Contains(query.Content));
    }

    if (query.IsPublished != null)
    {
      postQuery = postQuery.Where(p => p.IsPublished == query.IsPublished);
    }

    if (query.UserId.HasValue)
    {
      postQuery = postQuery.Where(p => p.UserId == query.UserId);
    }

    if (query.CreatedAt.HasValue)
    {
      postQuery = postQuery.Where(p => p.CreatedAt == query.CreatedAt);
    }

    if (query.PublishedAt.HasValue)
    {
      postQuery = postQuery.Where(p => p.PublishedAt == query.PublishedAt);
    }

    if (query.UpdatedAt.HasValue)
    {
      postQuery = postQuery.Where(p => p.UpdatedAt == query.UpdatedAt);
    }

    var total = await postQuery.CountAsync();
    var totalPages = (int)Math.Ceiling((double)total / query.Take);

    postQuery = postQuery
    .Skip((query.PageNumber - 1) * query.Take)
    .Take(query.Take);

    var posts = await postQuery.ToListAsync();
    return new GetAllDataDto<Post>
    {
      Data = posts,
      Total = total,
      TotalPages = totalPages,
      PageNumber = query.PageNumber
    };
  }

  async Task<Post?> IPostRepository.GetByIdAsync(int id)
  {
    return await _context.Posts.Include(p => p.User).Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
  }

  public async Task SaveChangesAsync()
  {
    await _context.SaveChangesAsync();
  }

  public async Task<Post?> UpdatePost(int id, Post post)
  {
    Post? foundPost = await _context.Posts
    .Include(p => p.User)
    .Include(p => p.Category)
    .FirstOrDefaultAsync(p => p.Id == id);

    if (foundPost == null) return null;
    foundPost.Title = post.Title;
    foundPost.Content = post.Content;
    foundPost.Slug = post.Slug;
    foundPost.CategoryId = post.CategoryId;
    foundPost.Category = await _context.Categories.FindAsync(post.CategoryId);
    foundPost.IsPublished = post.IsPublished;
    foundPost.PublishedAt = post.PublishedAt;
    foundPost.UpdatedAt = DateTime.Now;
    await _context.SaveChangesAsync();
    return post;
  }
}