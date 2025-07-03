namespace BlogAPI.Models;

public class Post(int id, string title, string content, string? slug, int userId, int categoryId)
{
  public required int Id { get; set; } = id;
  public required string Title { get; set; } = title;
  public required string Content { get; set; } = content;
  public string? Slug { get; set; } = slug;
  public required int UserId { get; set; } = userId;
  public User? User { get; set; }
  public int CategoryId { get; set; } = categoryId;
  public Category? Category { get; set; }
  public bool IsPublised { get; set; } = false;
  public DateTime CreatedAt { get; set; } = DateTime.Now;
  public DateTime? PublishedAt { get; set; }
  public DateTime UpdatedAt { get; set; } = DateTime.Now;

}
