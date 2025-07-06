using BlogAPI.DTOs;
using BlogAPI.Models;
namespace BlogAPI.Services;

public interface IPostService
{
  Task<GetAllDataDto<GetPostDetail>> GetAllPosts(PostQueryParamDto q);
  Task<GetPostDetail?> GetPostById(int id);
  Task<GetPostDetail> CreatePost(CreateUpdatePostRequest rq);
  Task<Post> UpdatePost(int id, CreateUpdatePostRequest rq);
}