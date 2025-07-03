using BlogAPI.DTOs;
using BlogAPI.Models;

namespace BlogAPI.Services;

public class PostService : IPostService
{
  public Task<Post> CreatePost(CreateUpdatePostRequest rq)
  {
    throw new NotImplementedException();
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
