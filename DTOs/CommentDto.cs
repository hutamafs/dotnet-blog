namespace BlogAPI.DTOs;

public class CreateUpdateCommentRequest
{
  public required string Text { get; set; }
  public int PostId { get; set; }
  public int UserId { get; set; }
}

public class GetCommentDetail
{
  public required int Id { get; set; }
  public required string Text { get; set; }
  public required string Author { get; set; }
  public required DateTime CommentAt { get; set; }
  public required DateTime UpdatedAt { get; set; }
  public int UserId { get; set; }
}
