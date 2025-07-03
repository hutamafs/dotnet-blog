namespace BlogAPI.Models;

public class User(string username, string email, string pw, string firstName, string lastName, string bio)
{
  public required int Id;
  public required string Username { get; set; } = username;
  public required string Email { get; set; } = email;
  public required string PasswordHash { get; set; } = pw;
  public required string Firstname { get; set; } = firstName;
  public string? Lastname { get; set; } = lastName;
  public string? Bio { get; set; } = bio;
  public DateTime CreatedAt { get; set; } = DateTime.Now;
  public DateTime UpdatedAt { get; set; } = DateTime.Now;

  public List<Post> Posts = [];

}
