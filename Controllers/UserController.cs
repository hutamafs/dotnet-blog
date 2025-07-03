using BlogAPI.DTOs;
using BlogAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers;

[ApiController]
[Route("api/users")]
public class UserController(IUserService service) : ControllerBase
{
  private readonly IUserService _service = service;

  [HttpPost]
  public async Task<IActionResult> Register(CreateUpdateUserRequest rq)
  {
    var user = await _service.CreateUser(rq);
    // return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);

    return Created("", user);
  }

}