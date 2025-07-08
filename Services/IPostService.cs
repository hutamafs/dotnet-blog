using BlogAPI.DTOs;
using BlogAPI.Models;
namespace BlogAPI.Services;

public interface IPostService
{
  Task<GetAllDataDto<GetPostDetail>> GetAllPosts(PostQueryParamDto q);
  Task<GetPostDetail?> GetPostById(int id);
  Task<Post?> GetPostModelById(int id);
  Task<GetPostDetail> CreatePost(CreatePostData rq);
  Task<GetPostDetail?> UpdatePost(int id, UpdatePostRequest rq);
  Task<bool?> UpdatePostStatus(int id, bool status);

  Task<GetAllDataDto<GetCommentDetail>> GetCommentsForPost(int id, CommentQueryParamDto query);

}