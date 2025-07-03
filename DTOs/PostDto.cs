namespace BlogAPI.DTOs;

public class CreateUpdatePostRequest
{
  public required string Title { get; set; }
  public required string Content { get; set; }
  public string? Slug { get; set; }
  public required int UserId { get; set; }

}

public class GetPostDetail
{
  public required int Id;
  public required string Title { get; set; }
  public required string Content { get; set; }
  public string? Slug { get; set; }
  public required int UserId { get; set; }
  public bool IsPublised { get; set; } = false;
  public DateTime CreatedAt { get; set; } = DateTime.Now;
  public DateTime PublishedAt { get; set; } = DateTime.Now;
  public DateTime UpdatedAt { get; set; } = DateTime.Now;


}