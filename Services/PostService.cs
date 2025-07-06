using BlogAPI.DTOs;
using BlogAPI.Models;
using BlogAPI.Repository;

namespace BlogAPI.Services;

public class PostService(IPostRepository postRepo, IUserRepository userRepo) : IPostService
{
  private readonly IPostRepository _postRepo = postRepo;
  private readonly IUserRepository _userRepo = userRepo;
  public async Task<GetPostDetail> CreatePost(CreateUpdatePostRequest rq)
  {
    try
    {
      var userExists = await _postRepo.IsUserIdExist(rq.UserId);

      if (!userExists)
      {
        throw new HttpException("NotFound", 404, "User not found");
      }

      if (rq.CategoryId.HasValue)
      {

        var categoryExists = await _postRepo.IsCategoryIdExist(rq.CategoryId.Value);
        if (!categoryExists)
        {
          throw new HttpException("NotFound", 404, "Category not found");
        }
      }

      Post post = new(rq.Title, rq.Content, rq.Slug, rq.UserId, rq.CategoryId);
      await _postRepo.CreatePostAsync(post);
      var user = await _userRepo.GetByIdAsync(post.UserId)!;

      return new GetPostDetail
      {
        Id = post.Id,
        Title = post.Title,
        Content = post.Content,
        Slug = post.Slug,
        UserId = post.UserId,
        Author = $"{user?.Firstname} {user?.Lastname}".Trim(),
        CategoryName = post.Category?.Name,
        IsPublished = post.IsPublished,
        CreatedAt = post.CreatedAt,
        UpdatedAt = post.UpdatedAt,
        PublishedAt = post.PublishedAt,
      };
    }
    catch (Exception ex)
    {
      throw new HttpException("error", 500, ex.Message);
    }
  }

  public async Task<GetAllDataDto<GetPostDetail>> GetAllPosts(PostQueryParamDto q)
  {
    try
    {
      var result = await _postRepo.GetAllPosts(q);
      var posts = result.Data.Select(p => new GetPostDetail
      {
        Id = p.Id,
        Title = p.Title,
        Content = p.Content,
        Slug = p.Slug,
        UserId = p.UserId,
        Author = $"{p.User?.Firstname} {p.User?.Lastname}".Trim(),
        CategoryName = p.Category?.Name,
        IsPublished = p.IsPublished,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt,
        PublishedAt = p.PublishedAt,
      });
      return new GetAllDataDto<GetPostDetail>
      {
        PageNumber = q.PageNumber,
        TotalPages = result.TotalPages,
        Data = posts,
        Total = result.Total,
      };
    }
    catch (Exception ex)
    {
      throw new HttpException("error", 500, ex.Message);
    }
  }

  public Task<GetPostDetail?> GetPostById(int id)
  {
    throw new NotImplementedException();
  }

  public Task<Post> UpdatePost(int id, CreateUpdatePostRequest rq)
  {
    throw new NotImplementedException();
  }
}
