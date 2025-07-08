using System.Security.Claims;
using BlogAPI.DTOs;
using BlogAPI.Helper;
using BlogAPI.Repository;
using BlogAPI.Services;
using BlogAPI.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers;

[ApiController]
[Route("api/comments")]
public class CommentController(ICommentRepository commentRepo, ICommentService service, IPostRepository postRepo) : ControllerBase
{
  private readonly ICommentService _service = service;
  private readonly ICommentRepository _commentRepo = commentRepo;
  private readonly IPostRepository _postRepo = postRepo;

  [Authorize]
  [HttpPost]
  public async Task<IActionResult> PostComment(CreateUpdateCommentRequest rq)
  {
    try
    {
      var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
      var validator = new CreateUpdateCommentValidator(_commentRepo, _postRepo);
      rq.UserId = userId;
      var validationResult = await validator.ValidateAsync(rq);

      if (!validationResult.IsValid)
      {
        foreach (var error in validationResult.Errors)
        {
          ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
        return BadRequest(ModelState);
      }
      var comment = await _service.PostComment(rq);
      var response = new ApiResponse<GetCommentDetail>(comment);
      return CreatedAtAction(nameof(GetSingleComment), new { id = comment.Id }, response);
    }
    catch (HttpException e)
    {
      return ErrorFormat.FormatErrorResponse(e.StatusCode, e.Title, e.Message, HttpContext);
    }
  }

  [Authorize]
  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetSingleComment(int id)
  {
    var comment = await _service.GetCommentById(id);
    if (comment == null) return NotFound();
    return Ok(new ApiResponse<GetCommentDetail>(comment));
  }

}