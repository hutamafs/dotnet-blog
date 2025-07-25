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
[Authorize]
public class CommentController(ICommentRepository commentRepo, ICommentService service, IPostRepository postRepo) : BaseApiController
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
      var userId = GetCurrentUserId();
      var validator = new CreateUpdateCommentValidator(_commentRepo, _postRepo);
      rq.UserId = userId;
      await ValidateRequest(rq, validator);
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

  [Authorize]
  [HttpPut("{id:int}")]
  public async Task<IActionResult> UpdateComment(int id, [FromBody] CreateUpdateCommentRequest rq)
  {
    try
    {
      var userId = GetCurrentUserId();
      var validator = new UpdateCommentValidator();
      await ValidateRequest(rq, validator);

      rq.UserId = userId;
      var foundComment = await _service.GetCommentById(id);
      if (foundComment == null) return NotFound();
      var result = CheckBelongings(foundComment.UserId);
      if (result != null) return result;
      var comment = await _service.UpdateCommentById(id, rq);
      return Ok(new ApiResponse<GetCommentDetail>(comment, 200, "successfully updated comment"));
    }
    catch (HttpException e)
    {
      return ErrorFormat.FormatErrorResponse(e.StatusCode, e.Title, e.Message, HttpContext);
    }

  }

  [Authorize(Roles = "Admin, User")]
  [HttpDelete("{id:int}")]
  public async Task<IActionResult> DeleteComment(int id)
  {
    try
    {
      var userId = GetCurrentUserId();
      var foundComment = await _service.GetCommentById(id);
      if (foundComment == null) return NotFound();
      var result = CheckBelongings(foundComment.UserId);
      if (result != null) return result;
      await _service.DeleteCommentById(id);
      return Ok(new ApiResponse<object>(null, 200, "successfully deleted comment"));
    }
    catch (HttpException e)
    {
      return ErrorFormat.FormatErrorResponse(e.StatusCode, e.Title, e.Message, HttpContext);
    }

  }

}