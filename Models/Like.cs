namespace BlogAPI.Models;

public class Like
{

  public Like() { }

  public Like(int userId, int postId)
  {
    UserId = userId;
    PostId = postId;
  }

  public required int UserId { get; set; }
  public User? User { get; set; }
  public Post? Post { get; set; }
  public required int PostId { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.Now;
  public DateTime UpdatedAt { get; set; } = DateTime.Now;

}
