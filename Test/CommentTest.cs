using Xunit;
using Moq;
using BlogAPI.Services;
using BlogAPI.Repository;
using BlogAPI.Models;
using BlogAPI.DTOs;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using Microsoft.IdentityModel.Tokens;
// using System.Text;

namespace BlogAPI.Test;

public class CommentTest
{
  private readonly Mock<ICommentRepository> _commentRepo;
  private readonly Mock<IUserRepository> _userRepo;
  private readonly ITestOutputHelper _output;
  private readonly CommentService _service;
  // private readonly JwtService _jwtService;


  public CommentTest(ITestOutputHelper output)
  {
    _commentRepo = new Mock<ICommentRepository>();
    _userRepo = new Mock<IUserRepository>();
    _service = new CommentService(_commentRepo.Object, _userRepo.Object);
    _output = output;

    // var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret-key-here-must-be-at-least-32-characters-long"));
    // _jwtService = new JwtService(_mockRepo.Object, secretKey);
  }

  private static CreateUpdateCommentRequest CreateCommentRequest()
  {
    return new CreateUpdateCommentRequest
    {
      Text = "text",
      PostId = 1,
      UserId = 1
    };
  }

  private static User CreateUserFromRequest(CreateUserRequest UserRequest)
  {
    var hasher = new PasswordHasher<User>();
    var hashedPassword = hasher.HashPassword(new User(), UserRequest.Password);

    return new User(UserRequest.Username, UserRequest.Email, hashedPassword, UserRequest.Firstname, UserRequest.Lastname, "");
  }

  private static User CreateSampleUser()
  {
    return new User { Id = 1, Email = "test@mail.com", Firstname = "hutama", Lastname = "saputra", Bio = "bio" };
  }

  private static Comment CreateSampleComment()
  {
    return new Comment
    {
      UserId = 1,
      PostId = 1,
      Text = "comment",
      User = CreateSampleUser()
    };
  }

  #region CreateComment Tests
  [Trait("Category", "CommentService")]
  [Trait("Method", "PostComment")]
  [Fact]
  public async Task PostCommentSuccessCase()
  {
    // Arrange
    var user = CreateSampleUser();
    var comment = CreateSampleComment();
    comment.User = CreateSampleUser();
    _commentRepo.Setup(repo => repo.IsPostIdExist(1)).ReturnsAsync(true);
    _commentRepo.Setup(repo => repo.PostCommentAsync(It.IsAny<Comment>())).ReturnsAsync(comment);

    // Act
    var result = await _service.PostComment(new CreateUpdateCommentRequest
    {
      Text = "text",
      PostId = 1,
      UserId = 1,
    });

    // Assert
    Assert.NotNull(result);
    Assert.Equal(comment.Text, result.Text);
    Assert.Equal(comment.UserId, result.UserId);
    Assert.Equal("hutama saputra", result.Author);

    _commentRepo.Verify(r => r.IsPostIdExist(comment.PostId), Times.Once);
  }

  #endregion

}