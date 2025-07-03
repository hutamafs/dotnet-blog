namespace BlogAPI.DTOs;

public class CreateCategoryRequest
{
  public required string Name { get; set; }
  public string? Description { get; set; }
  public string? Slug { get; set; }
}
