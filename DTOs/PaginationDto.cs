namespace BlogAPI.DTOs;

public class GetAllDataDto<T>
{
  public IEnumerable<T> Data { get; set; } = [];
  public int? Total { get; set; } = 0;
  public int? TotalPage { get; set; } = 0;
  public int? PageNumber { get; set; } = 0;
}
