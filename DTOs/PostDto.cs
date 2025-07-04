using BlogAPI.Models;

namespace BlogAPI.DTOs;

public class CreateUpdatePostRequest
{
  public required string Title { get; set; }
  public required string Content { get; set; }
  public string? Slug { get; set; }
  public required int UserId { get; set; }

  public int? CategoryId { get; set; }

}

public class GetPostDetail
{
  public required int Id { get; set; }
  public required string Title { get; set; }
  public required string Content { get; set; }
  public string? Slug { get; set; }
  public required int UserId { get; set; }
  public User? User { get; set; }
  public int? CategoryId { get; set; }
  public Category? Category { get; set; }
  public bool IsPublised { get; set; } = false;
  public DateTime? CreatedAt { get; set; }
  public DateTime? PublishedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }


}