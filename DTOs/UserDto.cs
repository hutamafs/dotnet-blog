namespace BlogAPI.DTOs;

public class CreateUpdateUserRequest
{
  public required string Username { get; set; }
  public required string Email { get; set; }
  public required string Password { get; set; }
  public required string Firstname { get; set; }
  public string? Lastname { get; set; }
  public string? Bio { get; set; }

}

public class GetUserDetail
{
  public required int Id { get; set; }
  public required string Email { get; set; }
  public required string Firstname { get; set; }
  public string? Lastname { get; set; }
  public string? Bio { get; set; }
  public required List<GetPostDetail> Posts { get; set; }
}
