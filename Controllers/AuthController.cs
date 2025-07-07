using BlogAPI.DTOs;
using BlogAPI.Services;
using Microsoft.AspNetCore.Mvc;
using BlogAPI.Validators;

namespace BlogAPI.Controllers;

[ApiController]
[Route("api/auth/login")]
public class AuthController(IJwtService service) : ControllerBase
{
  private readonly IJwtService _service = service;

  [HttpPost]
  public async Task<IActionResult> Register(LoginRequest rq)
  {
    var validator = new LoginValidator();
    var validationResult = await validator.ValidateAsync(rq);
    if (!validationResult.IsValid)
    {
      foreach (var error in validationResult.Errors)
      {
        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
      }
      return BadRequest(ModelState);
    }
    var token = await _service.LoginAndVerifyJwt(rq);
    return Ok(token);
  }

}