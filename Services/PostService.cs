using BlogAPI.DTOs;
using BlogAPI.Models;
using BlogAPI.Repository;
using BlogAPI.Helper;

namespace BlogAPI.Services;

public class PostService(IPostRepository postRepo, IUserRepository userRepo) : IPostService
{
  private readonly IPostRepository _postRepo = postRepo;
  private readonly IUserRepository _userRepo = userRepo;

  private async Task<bool> CheckUserExists(int id)
  {
    var userExists = await _postRepo.IsUserIdExist(id);
    if (!userExists)
    {
      throw new HttpException("NotFound", 404, "User not found");
    }
    return true;
  }

  private async Task<bool> CheckCategoryExists(int id)
  {
    var categoryExist = await _postRepo.IsCategoryIdExist(id);
    if (!categoryExist)
    {
      throw new HttpException("NotFound", 404, "Category not found");
    }
    return true;
  }

  public async Task<GetPostDetail> CreatePost(CreatePostData rq)
  {
    try
    {
      await CheckUserExists(rq.UserId);
      if (rq.CategoryId.HasValue)
      {
        await CheckCategoryExists(rq.CategoryId.Value);
      }

      Post post = new(rq.Title, rq.Content, rq.Slug, rq.UserId, rq.CategoryId);
      await _postRepo.CreatePostAsync(post);
      var user = await _userRepo.GetByIdAsync(rq.UserId);

      return FormatReturnPost.Map(post, user!);
    }
    catch (HttpException)
    {
      throw;
    }
    catch (Exception ex)
    {
      throw new HttpException("error", 500, ex.Message);
    }
  }

  public async Task<GetAllDataDto<GetPostDetail>> GetAllPosts(PostQueryParamDto? q)
  {
    try
    {
      q ??= new PostQueryParamDto();
      var result = await _postRepo.GetAllPosts(q);
      var posts = result.Data.Select(p => FormatReturnPost.Map(p, p.User!));
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

  public async Task<GetPostDetail?> GetPostById(int id)
  {
    Post? post = await _postRepo.GetByIdAsync(id);
    if (post == null) return null;
    return FormatReturnPost.Map(post, post.User!);
  }

  public async Task<GetPostDetail?> UpdatePost(int id, UpdatePostRequest rq)
  {
    try
    {
      var foundPost = await _postRepo.GetByIdAsync(id) ?? throw new HttpException("Not found", 404, "post not found");
      if (id != rq.UserId)
        throw new HttpException("Forbidden", 403, "you do not have access here");
      if (rq.CategoryId.HasValue)
      {
        await CheckCategoryExists(rq.CategoryId.Value);
      }

      Post post = new()
      {
        Title = rq.Title,
        Content = rq.Content,
        Slug = rq.Slug,
        UserId = foundPost.UserId,
        CategoryId = rq.CategoryId,
        IsPublished = rq.IsPublished,
        PublishedAt = rq.PublishedAt
      };
      var result = await _postRepo.UpdatePost(id, post);
      if (result == null) return null;
      return FormatReturnPost.Map(result, result.User!);
    }
    catch (HttpException)
    {
      throw;
    }
    catch
    {
      throw new HttpException("error", 500, "failed to update the post");
    }
  }

  public async Task<Post?> GetPostModelById(int id)
  {
    return await _postRepo.GetByIdAsync(id);
  }

  public async Task<bool?> UpdatePostStatus(int id, bool IsPublished, int userId)
  {
    try
    {
      var post = await _postRepo.GetByIdAsync(id);
      if (post == null) return null;
      if (post?.UserId != userId)
        throw new HttpException("Forbidden", 403, "you do not have access here");

      post.IsPublished = IsPublished;
      post.UpdatedAt = DateTime.Now;
      post.PublishedAt = DateTime.Now;
      await _postRepo.SaveChangesAsync();
      return post.IsPublished;
    }
    catch
    {
      throw new HttpException("error", 500, "failed to update post status");
    }
  }

  public async Task<GetAllDataDto<GetCommentDetail>> GetCommentsForPost(int id, CommentQueryParamDto query)
  {
    try
    {
      var response = await _postRepo.GetCommentsForPost(id, query);
      Console.WriteLine(response.Data);
      var data = response.Data.Select(FormatReturnComment.Map);
      return new GetAllDataDto<GetCommentDetail>
      {
        Data = data,
        PageNumber = 1,
        TotalPages = 1,
        Total = response.Total
      };
    }
    catch (System.Exception)
    {

      throw;
    }

  }
}
