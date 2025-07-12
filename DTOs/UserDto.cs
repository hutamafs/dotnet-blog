namespace BlogAPI.DTOs;

public class CreateUserRequest
{
  public required string Username { get; set; }
  public required string Email { get; set; }
  public required string Password { get; set; }
  public required string Firstname { get; set; }
  public string? Lastname { get; set; }
  public string? Bio { get; set; }

}

public class UpdateUserProfileRequest
{
  public required int UserId { get; set; }
  public required string Firstname { get; set; }
  public string? Lastname { get; set; }
  public string? Bio { get; set; }

}

public class UpdatePasswordRequest
{
  public required string Email { get; set; }
  public required string OldPassword { get; set; }
  public string? NewPassword { get; set; }

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

public class LoginRequest
{
  public required string Email { get; set; }
  public required string Password { get; set; }
}

public class LoginResponse
{
  public required string Email { get; set; }
  public required string Access_token { get; set; }
}

public class LikeByUserResponse
{
  public required string Username { get; set; }
  public required DateTime LikedAt { get; set; }
}
