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

  public async Task<IEnumerable<GetPostDetail>> GetAllPosts()
  {
    try
    {
      var posts = await _repo.GetAllPosts();
      return posts.Select(p => new GetPostDetail
      {
        Id = p.Id,
        UserId = p.UserId,
        User = p.User,
        Title = p.Title,
        Content = p.Content,
        Slug = p.Slug,
        Category = p.Category,
        CategoryId = p.CategoryId,
        IsPublised = p.IsPublised,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt,
        PublishedAt = p.PublishedAt
      });
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
