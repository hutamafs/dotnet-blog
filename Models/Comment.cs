namespace BlogAPI.Models;

public class Comment
{

  public Comment() { }

  public Comment(int userId, int postId, string text)
  {
    UserId = userId;
    PostId = postId;
    Text = text;
  }

  public int Id { get; set; }
  public required string Text { get; set; }
  public required int UserId { get; set; }
  public User? User { get; set; }
  public Post? Post { get; set; }
  public required int PostId { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.Now;
  public DateTime UpdatedAt { get; set; } = DateTime.Now;
  public bool IsDeleted { get; set; } = false;
  public DateTime? DeletedAt { get; set; }

}
