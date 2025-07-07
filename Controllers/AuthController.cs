using BlogAPI.DTOs;
using BlogAPI.Repository;
using BlogAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers;

[ApiController]
[Route("api/auth/login")]
public class AuthController(IUserRepository repo, IJwtService service) : ControllerBase
{
  private readonly IJwtService _service = service;
  private readonly IUserRepository _repo = repo;

  [HttpPost]
  public async Task<IActionResult> Register(LoginRequest rq)
  {
    var validator = new LoginValidator(_repo);
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