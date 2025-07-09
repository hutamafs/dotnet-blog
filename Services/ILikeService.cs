namespace BlogAPI.Services;

public interface ILikeService
{
  Task LikePost(int postId, int userId);
  Task UnlikePost(int postId, int userId);
}
