namespace BlogAPI.Controllers;

using System.Security.Claims;
using BlogAPI.Helper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

public abstract class BaseApiController : ControllerBase
{
  protected int GetCurrentUserId()
  {
    return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
  }

  protected string GetCurrentUserRole()
  {
    return User.FindFirst(ClaimTypes.Role)?.Value!;
  }

  protected async Task ValidateRequest<TRequest>(TRequest request, IValidator<TRequest> validator)
  {
    var validationResult = await validator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
      foreach (var error in validationResult.Errors)
      {
        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
      }
      throw new HttpException("Validation Error", 400, "Request validation failed");
    }
  }

  protected IActionResult? CheckBelongings(int resourceOwnerId)
  {
    if (resourceOwnerId != GetCurrentUserId() && GetCurrentUserRole() != "Admin") return ForbiddenPermissionFormat.ResponseFormat(HttpContext);
    return null;
  }

}