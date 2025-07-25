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

  async Task<GetAllDataDto<Post>> IPostRepository.GetAllPosts(PostQueryParamDto? query)
  {
    query ??= new PostQueryParamDto();

    // Set default pagination values
    var pageNumber = query.PageNumber > 0 ? query.PageNumber : 1;
    var take = query.Take > 0 ? query.Take : 10;

    IQueryable<Post> postQuery = _context.Posts
    .Include(p => p.Category)
    .Include(p => p.User)
    .Include(p => p.LikedByUser)
      .ThenInclude(l => l.User)
    .Include(p => p.Comments.Where(c => !c.IsDeleted))
    .Where(p => !p.IsPublished);

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
    return await _context.Posts
    .Include(p => p.User)
    .Include(p => p.Category)
    .Include(p => p.Comments)
      .ThenInclude(c => c.User)
    .Include(p => p.LikedByUser)
      .ThenInclude(l => l.User)
    .FirstOrDefaultAsync(p => p.Id == id);
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
    return foundPost;
  }

  public async Task<GetAllDataDto<Comment>> GetCommentsForPost(int id, CommentQueryParamDto query)
  {
    IQueryable<Comment> commentQuery = _context.Comments
    .Where(c => c.PostId == id).Include(c => c.User);

    var total = await commentQuery.CountAsync();
    commentQuery = commentQuery.Take(query.Take);

    var comments = await commentQuery.ToListAsync();
    return new GetAllDataDto<Comment>
    {
      Data = comments,
      Total = total,
      TotalPages = 1,
      PageNumber = 1
    };
  }
}