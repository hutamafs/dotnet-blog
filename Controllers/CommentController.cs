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
[Authorize]
public class CommentController(ICommentRepository commentRepo, ICommentService service, IPostRepository postRepo) : ControllerBase
{
  private readonly ICommentService _service = service;
  private readonly ICommentRepository _commentRepo = commentRepo;
  private readonly IPostRepository _postRepo = postRepo;
  private int GetCurrentUserId()
  {
    return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
  }

  private async Task ValidateCommentRequest(CreateUpdateCommentRequest rq, bool isUpdate)
  {
    FluentValidation.Results.ValidationResult validationResult;
    FluentValidation.IValidator<CreateUpdateCommentRequest> validator;
    if (isUpdate)
    {
      validator = new UpdateCommentValidator();
    }
    else
    {
      validator = new CreateUpdateCommentValidator(_commentRepo, _postRepo);
    }

    validationResult = await validator.ValidateAsync(rq);
    if (!validationResult.IsValid)
    {
      foreach (var error in validationResult.Errors)
      {
        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
      }
      throw new HttpException("Validation Error", 400, "Request validation failed");
    }
  }

  private static bool IsCommentOwner(GetCommentDetail comment, int userId)
  {
    return comment.UserId == userId;
  }

  [HttpPost]
  public async Task<IActionResult> PostComment(CreateUpdateCommentRequest rq)
  {
    try
    {
      var userId = GetCurrentUserId();
      var validator = new CreateUpdateCommentValidator(_commentRepo, _postRepo);
      rq.UserId = userId;
      await ValidateCommentRequest(rq, false);
      var comment = await _service.PostComment(rq);
      var response = new ApiResponse<GetCommentDetail>(comment);
      return CreatedAtAction(nameof(GetSingleComment), new { id = comment.Id }, response);
    }
    catch (HttpException e)
    {
      return ErrorFormat.FormatErrorResponse(e.StatusCode, e.Title, e.Message, HttpContext);
    }
  }

  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetSingleComment(int id)
  {
    var comment = await _service.GetCommentById(id);
    if (comment == null) return NotFound();
    return Ok(new ApiResponse<GetCommentDetail>(comment));
  }

  [HttpPut("{id:int}")]
  public async Task<IActionResult> UpdateComment(int id, [FromBody] CreateUpdateCommentRequest rq)
  {
    try
    {
      var userId = GetCurrentUserId();
      await ValidateCommentRequest(rq, true);

      rq.UserId = userId;
      var foundComment = await _service.GetCommentById(id);
      if (foundComment == null) return NotFound();
      if (!IsCommentOwner(foundComment, userId)) return ForbiddenPermissionFormat.ResponseFormat(HttpContext);
      var comment = await _service.UpdateCommentById(id, rq);
      return Ok(new ApiResponse<GetCommentDetail>(comment, 200, "successfully updated comment"));
    }
    catch (HttpException e)
    {
      return ErrorFormat.FormatErrorResponse(e.StatusCode, e.Title, e.Message, HttpContext);
    }

  }

  [HttpDelete("{id:int}")]
  public async Task<IActionResult> DeleteComment(int id)
  {
    try
    {
      var userId = GetCurrentUserId();
      var foundComment = await _service.GetCommentById(id);
      if (foundComment == null) return NotFound();
      if (!IsCommentOwner(foundComment, userId)) return ForbiddenPermissionFormat.ResponseFormat(HttpContext);
      await _commentRepo.DeleteComment(id);
      return Ok(new ApiResponse<object>(null, 200, "successfully deleted comment"));
    }
    catch (HttpException e)
    {
      return ErrorFormat.FormatErrorResponse(e.StatusCode, e.Title, e.Message, HttpContext);
    }

  }

}