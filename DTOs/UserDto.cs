using BlogAPI.Models;

namespace BlogAPI.DTOs;

public class CreateUpdateUserRequest
{
  public required string Username { get; set; }
  public required string Email { get; set; }
  public required string PasswordHash { get; set; }
  public required string Firstname { get; set; }
  public string? Lastname { get; set; }
  public string? Bio { get; set; }

}

public class GetUserDetail
{
  public required int Id;
  public required string Email { get; set; }
  public required string Firstname { get; set; }
  public string? Lastname { get; set; }
  public string? Bio { get; set; }
  public required List<Post> Posts;
}
