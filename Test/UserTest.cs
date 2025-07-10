using Xunit;
using Moq;
using BlogAPI.Services;
using BlogAPI.Repository;
using BlogAPI.Models;
using BlogAPI.DTOs;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BlogAPI.Test;

public class UserTest
{
  private readonly Mock<IUserRepository> _mockRepo;
  private readonly ITestOutputHelper _output;
  private readonly UserService _service;
  private readonly JwtService _jwtService;


  public UserTest(ITestOutputHelper output)
  {
    _mockRepo = new Mock<IUserRepository>();
    _service = new UserService(_mockRepo.Object);
    _output = output;

    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret-key-here-must-be-at-least-32-characters-long"));
    _jwtService = new JwtService(_mockRepo.Object, secretKey);
  }

  private static CreateUserRequest GetUserRequest()
  {
    return new CreateUserRequest
    {
      Username = "hutama",
      Email = "hutama@mail.com",
      Password = "hutama",
      Firstname = "hutama",
      Lastname = "saputra"
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
    return new User { Id = 1, Email = "test@mail.com", Firstname = "Test", Lastname = "User", Bio = "bio" };
  }

  #region GetUserDetail Tests
  [Fact]
  [Trait("Category", "UserService")]
  [Trait("Method", "GetUserDetail")]
  public async Task GetUserDetail_ReturnsUser_WhenUserExists()
  {
    // Arrange
    var user = CreateSampleUser();
    _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(user);

    // Act
    var result = await _service.GetUserDetail(1);

    // Assert
    Assert.NotNull(result);
    Assert.Equal("test@mail.com", result.Email);
  }

  [Fact]
  [Trait("Category", "UserService")]
  [Trait("Method", "GetUserDetail")]
  public async Task GetUserDetail_ReturnsNull_WhenUserNotExists()
  {
    var user = CreateSampleUser();
    _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(user);

    var result = await _service.GetUserDetail(2);

    Assert.Null(result);
  }

  #endregion

  #region RegisterUser Tests

  [Fact]
  [Trait("Category", "UserService")]
  [Trait("Method", "CreateUser")]
  public async Task CreateUser_ExistingEmail_Failed()
  {
    var UserRequest = GetUserRequest();
    _mockRepo.Setup(r => r.IsEmailTakenAsync(UserRequest.Email)).ReturnsAsync(true);
    var exception = await Assert.ThrowsAsync<HttpException>(() => _service.CreateUser(UserRequest));

    Assert.Equal(409, exception.StatusCode);
    Assert.Equal("Email already in use", exception.Message);
  }

  [Fact]
  [Trait("Category", "UserService")]
  [Trait("Method", "CreateUser")]
  public async Task CreateUser_ReturnsUserFormat_Successful()
  {
    var UserRequest = GetUserRequest();
    var user = CreateUserFromRequest(UserRequest);
    _mockRepo.Setup(r => r.IsEmailTakenAsync(UserRequest.Email)).ReturnsAsync(false);
    _mockRepo.Setup(r => r.IsUsernameTakenAsync(UserRequest.Username)).ReturnsAsync(false);
    _mockRepo.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
    _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

    var result = await _service.CreateUser(UserRequest);

    Assert.NotNull(result);
    Assert.Equal(UserRequest.Email, result.Email);
    Assert.Equal(UserRequest.Firstname, result.Firstname);
    Assert.Equal(UserRequest.Lastname, result.Lastname);

    _mockRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
  }

  #endregion

  #region UpdateUser Tests
  [Trait("Category", "UserService")]
  [Trait("Method", "UpdatePersonalDetail")]
  [Fact]
  public async Task UpdatePersonalDetail_Successful()
  {
    var userId = 1;
    var user = CreateSampleUser();
    var request = new UpdateUserProfileRequest { UserId = 1, Firstname = "new name" };
    _mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
    _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

    var result = await _service.UpdateUser(userId, request);

    Assert.NotNull(result);
    Assert.Equal(user.Firstname, result.Firstname);
    _mockRepo.Verify(r => r.GetByIdAsync(userId), Times.Once);
    _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
  }

  [Fact]
  public async Task UpdatePersonalDetail_FailedForbidden()
  {
    var user = CreateSampleUser();
    var request = new UpdateUserProfileRequest { UserId = 1, Firstname = "new name" };
    _mockRepo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(user);
    var exception = await Assert.ThrowsAsync<HttpException>(() => _service.UpdateUser(3, request));

    Assert.Equal(403, exception.StatusCode);
  }

  #endregion

  #region Login Test
  [Trait("Category", "UserService")]
  [Trait("Method", "Login")]
  [Fact]
  public async Task LoginSuccessful()
  {
    var request = new LoginRequest
    {
      Email = "test@mail.com",
      Password = "hutama"
    };
    var user = CreateSampleUser();
    var hasher = new PasswordHasher<User>();
    user.PasswordHash = hasher.HashPassword(user, request.Password);
    _mockRepo.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync(user);

    var result = await _jwtService.LoginAndVerifyJwt(request);

    Assert.NotNull(result);
    Assert.False(string.IsNullOrEmpty(result.Access_token));

    var handler = new JwtSecurityTokenHandler();
    var jwt = handler.ReadJwtToken(result.Access_token);
    var emailClaim = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

    Assert.Equal(request.Email, emailClaim);
  }

  #endregion

}