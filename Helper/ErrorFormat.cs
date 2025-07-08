using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Helper;

public class ErrorFormat
{
  public static ObjectResult FormatErrorResponse(int statusCode, string title, string detail, HttpContext context)
  {
    return new ObjectResult(new ProblemDetails
    {
      Title = title,
      Status = statusCode,
      Detail = detail,
      Instance = context.Request.Path
    })
    {
      StatusCode = statusCode,
      ContentTypes = { "application/json" }
    };
  }
}