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
[Route("api/posts")]
public class PostController(IPostRepository repo, ILikeService likeService, IPostService service) : BaseApiController
{
  private readonly IPostService _service = service;
  private readonly IPostRepository _repo = repo;
  private readonly ILikeService _likeService = likeService;

  [Authorize]
  [HttpPost]
  public async Task<IActionResult> CreatePost(CreatePostRequest rq)
  {
    var userId = GetCurrentUserId();
    var validator = new CreatePostValidator(_repo);
    var postData = new CreatePostData
    {
      Title = rq.Title,
      Content = rq.Content,
      Slug = rq.Slug,
      UserId = userId,
      CategoryId = rq.CategoryId
    };

    await ValidateRequest(postData, validator);

    var post = await _service.CreatePost(postData);
    var response = new ApiResponse<GetPostDetail>(post);
    return CreatedAtAction(nameof(GetPostDetail), new { id = post.Id }, response);
  }

  [HttpGet]
  public async Task<IActionResult> GetAllPosts([FromQuery] PostQueryParamDto query)
  {
    try
    {
      var posts = await _service.GetAllPosts(query);
      return Ok(new ApiResponse<GetAllDataDto<GetPostDetail>>(posts));
    }
    catch (HttpException e)
    {
      return ErrorFormat.FormatErrorResponse(e.StatusCode, e.Title, e.Message, HttpContext);
    }
  }

  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetPostDetail(int id)
  {
    var post = await _service.GetPostById(id);
    if (post == null) return NotFound();
    return Ok(new ApiResponse<GetPostDetail>(post));
  }

  [HttpGet("{id:int}/comments")]
  public async Task<IActionResult> GetComments(int id, [FromQuery] CommentQueryParamDto query)
  {
    var comments = await _service.GetCommentsForPost(id, query);
    return Ok(new ApiResponse<GetAllDataDto<GetCommentDetail>>(comments));
  }

  [Authorize]
  [HttpPost("{id:int}/like")]
  public async Task<IActionResult> LikePost(int id)
  {
    await _likeService.LikePost(id, int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value));
    return Ok(new ApiResponse<object>(null, 200, "post has been liked"));
  }

  [Authorize]
  [HttpDelete("{id:int}/like")]
  public async Task<IActionResult> UnlikePost(int id)
  {
    await _likeService.UnlikePost(id, int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value));
    return Ok(new ApiResponse<object>(null, 200, "post has been unliked"));
  }

  [Authorize]
  [HttpPut("{id:int}")]
  public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostRequest rq)
  {
    try
    {
      var userId = GetCurrentUserId();
      var validator = new UpdatePostValidator(_repo);
      await ValidateRequest(rq, validator);
      rq.UserId = userId;
      var post = await _service.UpdatePost(id, rq);
      if (post == null) return NotFound();
      var result = CheckBelongings(post.UserId);
      if (result != null) return result;
      return Ok(new ApiResponse<GetPostDetail>(post));
    }
    catch (HttpException e)
    {
      return ErrorFormat.FormatErrorResponse(e.StatusCode, e.Title, e.Message, HttpContext);
    }
  }

  [Authorize]
  [HttpPatch("{id:int}")]
  public async Task<IActionResult> UpdatePostStatus(int id, [FromBody] UpdatePostStatusRequest rq)
  {
    try
    {
      var userId = GetCurrentUserId();
      var foundPost = await _service.GetPostById(id);
      if (foundPost == null) return NotFound();
      var success = await _service.UpdatePostStatus(id, rq.IsPublished, userId);
      if (success == null) return NotFound();
      return Ok(new ApiResponse<object>(null, 200, $"Post status updated to  {(success == true ? "published" : "unpublished")}"));
    }
    catch (HttpException e)
    {
      return ErrorFormat.FormatErrorResponse(e.StatusCode, e.Title, e.Message, HttpContext);
    }
  }
}