using BlogAPI.DTOs;
using BlogAPI.Repository;
using BlogAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers;

[ApiController]
[Route("api/posts")]
public class PostController(IPostRepository repo, IPostService service) : ControllerBase
{
  private readonly IPostService _service = service;
  private readonly IPostRepository _repo = repo;

  [HttpPost]
  public async Task<IActionResult> CreatePost(CreateUpdatePostRequest rq)
  {
    var validator = new PostValidator(_repo);
    var validationResult = await validator.ValidateAsync(rq);
    if (!validationResult.IsValid)
    {
      foreach (var error in validationResult.Errors)
      {
        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
      }
      return BadRequest(ModelState);
    }

    var post = await _service.CreatePost(rq);
    // return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);

    return Created("", post);
  }

  // [HttpGet("{id}")]

  // public async Task<IActionResult> GetUserDetail(int id)
  // {
  //   var user = await _service.GetUserDetail(id);
  //   if (user == null) return NotFound();
  //   return Ok(user);
  // }

  [HttpGet]

  public async Task<IActionResult> GetAllPosts()
  {
    var posts = await _service.GetAllPosts();
    return Ok(posts);
  }

}