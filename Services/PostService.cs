using BlogAPI.DTOs;
using BlogAPI.Models;
using BlogAPI.Repository;

namespace BlogAPI.Services;

public class PostService(IPostRepository repo) : IPostService
{
  private readonly IPostRepository _repo = repo;
  public async Task<Post> CreatePost(CreateUpdatePostRequest rq)
  {
    try
    {
      var userExists = await _repo.IsUserIdExist(rq.UserId);

      if (!userExists)
      {
        throw new HttpException("NotFound", 404, "User not found");
      }

      if (rq.CategoryId.HasValue)
      {

        var categoryExists = await _repo.IsCategoryIdExist(rq.CategoryId.Value);
        if (!categoryExists)
        {
          throw new HttpException("NotFound", 404, "Category not found");
        }
      }

      Post post = new(rq.Title, rq.Content, rq.Slug, rq.UserId, rq.CategoryId);
      await _repo.CreatePostAsync(post);
      return post;
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
      var result = await _repo.GetAllPosts(q);
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
