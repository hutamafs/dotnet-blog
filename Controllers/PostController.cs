using System.Security.Claims;
using BlogAPI.DTOs;
using BlogAPI.Helper;
using BlogAPI.Repository;
using BlogAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers;

[ApiController]
[Route("api/posts")]
public class PostController(IPostRepository repo, IPostService service) : ControllerBase
{
  private readonly IPostService _service = service;
  private readonly IPostRepository _repo = repo;

  [Authorize]
  [HttpPost]
  public async Task<IActionResult> CreatePost(CreatePostRequest rq)
  {
    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    var validator = new CreatePostValidator(_repo);
    var postData = new CreatePostData
    {
      Title = rq.Title,
      Content = rq.Content,
      Slug = rq.Slug,
      UserId = userId,
      CategoryId = rq.CategoryId
    };

    var validationResult = await validator.ValidateAsync(postData);
    if (!validationResult.IsValid)
    {
      foreach (var error in validationResult.Errors)
      {
        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
      }
      return BadRequest(ModelState);
    }

    var post = await _service.CreatePost(postData);
    return CreatedAtAction(nameof(GetPostDetail), new { id = post.Id }, post);
  }

  [HttpGet]
  public async Task<IActionResult> GetAllPosts([FromQuery] PostQueryParamDto query)
  {
    try
    {
      var posts = await _service.GetAllPosts(query);
      return Ok(posts);
    }
    catch (HttpException httpEx)
    {

      return StatusCode(httpEx.StatusCode, new ProblemDetails
      {
        Title = httpEx.Title,
        Status = httpEx.StatusCode,
        Detail = httpEx.Message,
        Instance = HttpContext.Request.Path
      });
    }

  }

  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetPostDetail(int id)
  {
    var post = await _service.GetPostById(id);
    if (post == null) return NotFound();
    return Ok(post);
  }

  [Authorize]
  [HttpPut("{id:int}")]
  public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostRequest rq)
  {
    try
    {
      var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
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
      if (post.UserId != userId) return ForbiddenPermissionFormat.ResponseFormat(HttpContext);
      return AcceptedAtAction(nameof(GetPostDetail), new { id = post.Id }, post);
    }
    catch (HttpException e)
    {
      return ErrorFormat.FormatErrorResponse(e.StatusCode, e.Title, e.Message, HttpContext);
    }

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