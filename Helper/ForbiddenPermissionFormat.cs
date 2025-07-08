using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Helper;

public static class ForbiddenPermissionFormat
{
  public static IActionResult ResponseFormat(HttpContext context)
  {
    return ErrorFormat.FormatErrorResponse(403, "Forbidden", "You do not have permission to update this.", context);
  }
}
