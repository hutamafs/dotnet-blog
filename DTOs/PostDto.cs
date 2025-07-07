namespace BlogAPI.DTOs;

public class CreatePostRequest
{
  public required string Title { get; set; }
  public required string Content { get; set; }
  public string? Slug { get; set; }

  public int? CategoryId { get; set; }
}

public class CreatePostData
{
  public required string Title { get; set; }
  public required string Content { get; set; }
  public string? Slug { get; set; }
  public required int UserId { get; set; }
  public int? CategoryId { get; set; }
}

public class UpdatePostRequest
{
  public required int UserId { get; set; }
  public required string Title { get; set; }
  public required string Content { get; set; }
  public string? Slug { get; set; }
  public int? CategoryId { get; set; }
  public bool IsPublished { get; set; }

  public DateTime? PublishedAt { get; set; }
}

public class GetPostDetail
{
  public required int Id { get; set; }
  public required string Title { get; set; }
  public required string Content { get; set; }
  public string? Slug { get; set; }
  public required int UserId { get; set; }
  public string? Author { get; set; }
  public int? CategoryId { get; set; }
  public string? CategoryName { get; set; }
  public bool IsPublished { get; set; } = false;
  public DateTime? CreatedAt { get; set; }
  public DateTime? PublishedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }

}

public class UpdatePostStatusRequest
{
  public required bool IsPublished { get; set; }
}