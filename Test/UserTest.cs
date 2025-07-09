using Xunit;
using Moq;
using BlogAPI.Services;
using BlogAPI.Repository;
using BlogAPI.Models;
using BlogAPI.DTOs;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace BlogAPI.Tests;

public class UserTest
{
  private readonly Mock<IUserRepository> _mockRepo;
  private readonly ITestOutputHelper _output;
  private readonly UserService _service;

  public UserTest(ITestOutputHelper output)
  {
    _mockRepo = new Mock<IUserRepository>();
    _service = new UserService(_mockRepo.Object);
    _output = output;
  }

  #region GetUserDetail Tests
  [Fact]
  [Trait("Category", "UserService")]
  [Trait("Method", "GetUserDetail")]
  public async Task GetUserDetail_ReturnsUser_WhenUserExists()
  {
    // Arrange
    var user = new User { Id = 1, Email = "test@mail.com", Firstname = "Test", Lastname = "User", Bio = "bio" };
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
    var user = new User { Id = 1, Email = "test@mail.com", Firstname = "Test", Lastname = "User", Bio = "bio" };
    _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(user);

    var result = await _service.GetUserDetail(2);

    Assert.Null(result);
  }

  #endregion

  #region RegisterUser Tests
  [Fact]
  [Trait("Category", "UserService")]
  [Trait("Method", "CreateUser")]
  public async Task CreateUser_ReturnsUserFormat_Successful()
  {
    var UserRequest = new CreateUserRequest
    {
      Username = "hutama",
      Email = "hutama@mail.com",
      Password = "hutama",
      Firstname = "hutama",
      Lastname = "saputra"
    };
    var hasher = new PasswordHasher<User>();
    var hashedPassword = hasher.HashPassword(new User(), UserRequest.Password);
    var user = new User(UserRequest.Username, UserRequest.Email, hashedPassword, UserRequest.Firstname, UserRequest.Lastname, "");
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
}