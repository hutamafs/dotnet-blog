using BlogAPI.DTOs;
using BlogAPI.Repository;
using BlogAPI.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers;

[ApiController]
[Route("api/posts")]
public class PostController(IPostRepository repo, IPostService service) : ControllerBase
{
  private readonly IPostService _service = service;
  private readonly IPostRepository _repo = repo;

  [HttpPost]
  public async Task<IActionResult> CreatePost(CreatePostRequest rq)
  {
    var validator = new CreatePostValidator(_repo);
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
    return CreatedAtAction(nameof(GetPostDetail), new { id = post.Id }, post);
  }

  [HttpGet]
  public async Task<IActionResult> GetAllPosts([FromQuery] PostQueryParamDto query)
  {
    var posts = await _service.GetAllPosts(query);
    return Ok(posts);
  }

  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetPostDetail(int id)
  {
    var post = await _service.GetPostById(id);
    if (post == null) return NotFound();
    return Ok(post);
  }

  [HttpPut("{id:int}")]
  public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostRequest rq)
  {
    var validator = new UpdatePostValidator(_repo);
    var validationResult = await validator.ValidateAsync(rq);
    if (!validationResult.IsValid)
    {
      foreach (var error in validationResult.Errors)
      {
        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
      }
      return BadRequest(ModelState);
    }
    var post = await _service.UpdatePost(id, rq);
    if (post == null) return NotFound();
    return AcceptedAtAction(nameof(GetPostDetail), new { id = post.Id }, post);
  }

  [HttpPatch("{id:int}")]
  public async Task<IActionResult> UpdatePostStatus(int id, [FromBody] UpdatePostStatusRequest rq)
  {
    try
    {
      var success = await _service.UpdatePostStatus(id, rq.IsPublished);
      if (success == null) return NotFound();
      return NoContent();
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { error = ex.Message });
    }
  }

}