using BlogAPI.Models;
using BlogAPI.Repository;

namespace BlogAPI.Services;

public class LikeService(ILikeRepository likeRepo) : ILikeService
{
  private readonly ILikeRepository _likeRepo = likeRepo;

  public async Task LikePost(int postId, int userId)
  {
    try
    {
      var like = new Like
      {
        PostId = postId,
        UserId = userId,
      };
      if (!await _likeRepo.FindLike(like))
      {
        await _likeRepo.LikePost(like);
        return;
      }
      throw new HttpException("Bad request", 400, "you have liked this post");
    }
    catch (System.Exception)
    {

      throw;
    }

  }

  public async Task UnlikePost(int postId, int userId)
  {
    try
    {
      var like = new Like
      {
        PostId = postId,
        UserId = userId,
      };
      if (await _likeRepo.FindLike(like))
      {
        await _likeRepo.UnlikePost(like);
        return;
      }
      throw new HttpException("Bad request", 400, "you have not liked this post");
    }
    catch (System.Exception)
    {

      throw;
    }

  }
}
