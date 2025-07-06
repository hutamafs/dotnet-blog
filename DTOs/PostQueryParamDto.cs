namespace BlogAPI.DTOs;

using BlogAPI.Models;

public class PostQueryParamDto
{

  public int PageNumber { get; set; } = 1;
  public int Take { get; set; } = 10;
  public string? Q { get; set; }
  public string? Search { get; set; }
  public int? Id { get; set; }
  public string? Title { get; set; }
  public string? Content { get; set; }
  public string? Slug { get; set; }
  public int? UserId { get; set; }
  public User? User { get; set; }
  public int? CategoryId { get; set; }
  public Category? Category { get; set; }
  public bool? IsPublished { get; set; }
  public DateTime? CreatedAt { get; set; }
  public DateTime? PublishedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }

}