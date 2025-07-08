namespace BlogAPI.Helper;

public class ApiResponse<T>(T? data, int status = 200, string? message = "success")
{
  public T? Data { get; set; } = data;
  public int? Status { get; set; } = status;
  public string? Message { get; set; } = message;
}
