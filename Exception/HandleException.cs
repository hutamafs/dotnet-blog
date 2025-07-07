using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
public class HttpException : Exception
{
  public int StatusCode { get; }
  public string Title { get; }

  public HttpException(string title, int statusCode, string message)
      : base(message)
  {
    Title = title;
    StatusCode = statusCode;
  }
}

public class ExceptionFilter : IExceptionFilter
{
  public void OnException(ExceptionContext context)
  {
    var ex = context.Exception;

    string title;
    int status;
    string message;

    if (ex is HttpException httpEx)
    {
      title = httpEx.Title;
      status = httpEx.StatusCode;
      message = httpEx.Message;
    }
    else
    {
      title = "Internal Server Error";
      status = 500;
      message = ex.Message;
    }

    context.Result = new ObjectResult(new ProblemDetails
    {
      Title = title,
      Status = status,
      Detail = message,
      Instance = context.HttpContext.Request.Path
    })
    {
      StatusCode = status,
      ContentTypes = { "application/json" }
    };

    context.ExceptionHandled = true;
  }
}