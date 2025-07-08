namespace BlogAPI.Models;

public class Post
{
  public Post() { }

  public Post(string title, string content, string? slug, int userId, int? categoryId)
  {
    Title = title;
    Content = content;
    Slug = slug;
    UserId = userId;
    CategoryId = categoryId;
  }

  public int Id { get; set; }
  public string Title { get; set; } = default!;
  public string Content { get; set; } = default!;
  public string? Slug { get; set; } = "";
  public int UserId { get; set; } = default!;
  public User? User { get; set; }
  public int? CategoryId { get; set; }
  public Category? Category { get; set; }
  public bool IsPublished { get; set; } = false;
  public DateTime CreatedAt { get; set; } = DateTime.Now;
  public DateTime? PublishedAt { get; set; }
  public DateTime UpdatedAt { get; set; } = DateTime.Now;
  public List<Like> LikedByUser { get; set; } = [];
}
