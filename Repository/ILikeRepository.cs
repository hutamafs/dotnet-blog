using BlogAPI.Models;

namespace BlogAPI.Repository;

public interface ILikeRepository
{
  Task<bool> FindLike(Like like);
  Task LikePost(Like like);
  Task UnlikePost(Like like);

}