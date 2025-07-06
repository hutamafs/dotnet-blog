using BlogAPI.DTOs;
using BlogAPI.Models;
namespace BlogAPI.Services;

public interface IPostService
{
  Task<GetAllDataDto<GetPostDetail>> GetAllPosts(PostQueryParamDto q);
  Task<GetPostDetail?> GetPostById(int id);
  Task<GetPostDetail> CreatePost(CreatePostRequest rq);
  Task<GetPostDetail?> UpdatePost(int id, UpdatePostRequest rq);
}