using System.Security.Claims;
using BlogAPI.DTOs;
using BlogAPI.Repository;
using BlogAPI.Services;
using BlogAPI.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers;

[ApiController]
[Route("api/users")]
public class UserController(IUserRepository repo, IUserService service) : ControllerBase
{
  private readonly IUserService _service = service;
  private readonly IUserRepository _repo = repo;

  [HttpPost]
  public async Task<IActionResult> Register(CreateUserRequest rq)
  {
    var validator = new CreateUserValidator(_repo);
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

  [HttpGet("me")]

  public async Task<IActionResult> GetOwnDetail()
  {
    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    var user = await _service.GetUserDetail(userId);
    if (user == null) return NotFound();
    return Ok(user);
  }

  [Authorize]
  [HttpPut("me")]
  public async Task<IActionResult> UpdateSelfDetail([FromBody] UpdateUserProfileRequest rq)
  {
    try
    {
      var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
      var validator = new UpdateUserProfileValidator();
      var validationResult = await validator.ValidateAsync(rq);
      if (!validationResult.IsValid)
      {
        foreach (var error in validationResult.Errors)
        {
          ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
        return BadRequest(ModelState);
      }

      var user = await _service.UpdateUser(userId, rq);
      if (user == null) return NotFound();
      if (user.Id != userId)
      {
        return StatusCode(403, new ProblemDetails
        {
          Title = "Forbidden",
          Status = 403,
          Detail = "You do not have permission to update this profile.",
          Instance = HttpContext.Request.Path
        });
      }
      return AcceptedAtAction(nameof(GetOwnDetail), new { id = userId }, user);
    }
    catch (HttpException e)
    {
      return StatusCode(e.StatusCode, new ProblemDetails
      {
        Title = e.Title,
        Status = e.StatusCode,
        Detail = e.Message,
        Instance = HttpContext.Request.Path
      });
    }

  }
}