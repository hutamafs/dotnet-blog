using BlogAPI.DTOs;
using BlogAPI.Repository;
using BlogAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers;

[ApiController]
[Route("api/users")]
public class UserController(IUserRepository repo, IUserService service) : ControllerBase
{
  private readonly IUserService _service = service;
  private readonly IUserRepository _repo = repo;

  [HttpPost]
  public async Task<IActionResult> Register(CreateUpdateUserRequest rq)
  {
    var validator = new CreateUpdateValidator(_repo);
    var validationResult = await validator.ValidateAsync(rq);
    if (!validationResult.IsValid)
    {
      foreach (var error in validationResult.Errors)
      {
        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
      }
      return BadRequest(ModelState);
    }
    var user = await _service.CreateUser(rq);
    // return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);

    return Created("", user);
  }

  [HttpGet("{id}")]

  public async Task<IActionResult> GetUserDetail(int id)
  {
    var user = await _service.GetUserDetail(id);
    if (user == null) return NotFound();
    return Ok(user);
  }

}