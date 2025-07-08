namespace BlogAPI.Models;

public class User
{

  public User() { }

  public User(string username, string email, string pw, string firstName, string? lastName, string? bio)
  {
    Username = username;
    Email = email;
    PasswordHash = pw;
    Firstname = firstName;
    Lastname = lastName;
    Bio = bio;
  }

  public int Id { get; set; }
  public string Username { get; set; } = default!;
  public string Email { get; set; } = default!;
  public string PasswordHash { get; set; } = default!;
  public string Firstname { get; set; } = default!;
  public string? Lastname { get; set; } = "";
  public string? Bio { get; set; } = "";
  public DateTime CreatedAt { get; set; } = DateTime.Now;
  public DateTime UpdatedAt { get; set; } = DateTime.Now;

  public List<Post> Posts { get; set; } = [];
  public List<Like> LikedPosts { get; set; } = [];

}
