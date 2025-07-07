using System.Security.Claims;

namespace BlogAPI.Helpers
{
  public static class ClaimsPrincipalExtensions
  {
    public static int GetUserId(this ClaimsPrincipal user)
    {
      return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }

    public static string? GetEmail(this ClaimsPrincipal user)
    {
      return user.FindFirst(ClaimTypes.Email)?.Value;
    }
  }
}