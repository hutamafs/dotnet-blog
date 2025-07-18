namespace BlogAPI.Models;

public class Category
{
  public required int Id { get; set; }
  public required string Name { get; set; }
  public string? Description { get; set; }
  public string? Slug { get; set; }
}
