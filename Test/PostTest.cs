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

public class PostTest
{
  private readonly Mock<IPostRepository> _postRepo;
  private readonly Mock<IUserRepository> _userRepo;
  private readonly ITestOutputHelper _output;
  private readonly PostService _service;
  // private readonly JwtService _jwtService;


  public PostTest(ITestOutputHelper output)
  {
    _postRepo = new Mock<IPostRepository>();
    _userRepo = new Mock<IUserRepository>();
    _service = new PostService(_postRepo.Object, _userRepo.Object);
    _output = output;

    // var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret-key-here-must-be-at-least-32-characters-long"));
    // _jwtService = new JwtService(_mockRepo.Object, secretKey);
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

  private static User CreateSampleUser()
  {
    return new User { Id = 1, Email = "test@mail.com", Firstname = "hutama", Lastname = "saputra", Bio = "bio" };
  }

  private static Post CreateSamplePost()
  {
    return new Post
    {
      Id = 1,
      Title = "title",
      Content = "Content",
      UserId = 1,
      CategoryId = 1,
      Slug = "sample-slug",
      Category = new Category
      {
        Id = 1,
        Name = "category"
      },

    };
  }

  #region CreatePost Tests
  [Trait("Category", "PostService")]
  [Trait("Method", "CreatePost")]
  [Fact]
  public async Task CreatePost_SuccessCase()
  {
    // Arrange
    var post = CreateSamplePost();
    var user = CreateSampleUser();
    _userRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(user);
    _postRepo.Setup(repo => repo.IsUserIdExist(1)).ReturnsAsync(true);
    _postRepo.Setup(repo => repo.IsCategoryIdExist(1)).ReturnsAsync(true);
    _postRepo.Setup(repo => repo.CreatePostAsync(post));

    var postData = new CreatePostData
    {
      Title = "title",
      Content = "Content",
      UserId = 1,
      CategoryId = 1,
    };

    // Act
    var result = await _service.CreatePost(postData);

    // Assert
    Assert.NotNull(result);
    Assert.Equal("title", result.Title);
    Assert.Equal("Content", result.Content);
    Assert.Equal(1, result.UserId);

    _postRepo.Verify(r => r.IsUserIdExist(1), Times.Once);
    _userRepo.Verify(r => r.GetByIdAsync(1), Times.Once);
    _postRepo.Verify(r => r.CreatePostAsync(It.IsAny<Post>()), Times.Once);
  }

  [Fact]
  public async Task CreatePost_FailedCase()
  {
    // Arrange
    _postRepo.Setup(repo => repo.IsCategoryIdExist(1)).ReturnsAsync(false);
    _postRepo.Setup(repo => repo.IsCategoryIdExist(1)).ReturnsAsync(true);

    var postData = new CreatePostData
    {
      Title = "title",
      Content = "Content",
      UserId = 1,
      CategoryId = 1,
    };

    var exception = await Assert.ThrowsAsync<HttpException>(() => _service.CreatePost(postData));

    // Assert
    Assert.Equal(404, exception.StatusCode);
    Assert.Equal("User not found", exception.Message);

  }

  #endregion

  #region GetPostDetail Tests
  [Fact]
  [Trait("Category", "PostService")]
  [Trait("Method", "GetPostDetail")]
  public async Task GetPostDetail_SuccessCase()
  {
    // Arrange
    var user = CreateSampleUser();
    var post = CreateSamplePost();
    post.User = user;
    _postRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);
    _userRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

    // Act
    var result = await _service.GetPostById(1);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(1, result.Id);
    Assert.Equal(1, result.UserId);
    Assert.Equal("title", result.Title);
    Assert.Equal("sample-slug", result.Slug);
    Assert.Equal("hutama saputra", result.Author);
    Assert.Equal("category", result.CategoryName);

    _postRepo.Verify(r => r.GetByIdAsync(1), Times.Once);
  }

  [Fact]
  public async Task GetPostDetail_FailedCase()
  {
    var post = CreateSamplePost();
    _postRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);
    var result = await _service.GetPostById(999);

    Assert.Null(result);
    _postRepo.Verify(r => r.GetByIdAsync(999), Times.Once);
  }

  #endregion

  #region GetAllPosts Tests
  [Fact]
  [Trait("Category", "PostService")]
  [Trait("Method", "GetAllPosts")]
  public async Task GetAllPostsSuccessCase()
  {
    // Arrange
    var posts = new List<Post>
    {
      new () { Id = 1, Title = "Post 1", IsPublished = true, UserId=1, Content="content",  CategoryId = 1, Comments=[] },
      new () { Id = 2, Title = "Post 2", IsPublished = true, UserId=1, Content="content",  CategoryId = 2 },
    };

    var param = new PostQueryParamDto { Take = 2 };

    _postRepo.Setup(r => r.GetAllPosts(param)).ReturnsAsync(new GetAllDataDto<Post>
    {
      PageNumber = 1,
      TotalPages = 1,
      Total = 2,
      Data = posts
    });

    // Act
    var result = await _service.GetAllPosts(param);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(posts.Count, result.Data.Count());
    Assert.Equal(posts[0].Title, result.Data.ToList()[0].Title);
    Assert.Equal(posts[0].Content, result.Data.ToList()[0].Content);
    Assert.Equal(posts[0].UserId, result.Data.ToList()[0].UserId);

    _postRepo.Verify(r => r.GetAllPosts(param), Times.Once);
  }

  [Fact]
  [Trait("Category", "PostService")]
  [Trait("Method", "GetAllPosts_EmptyResult")]
  public async Task GetAllPosts_EmptyResult()
  {

    var param = new PostQueryParamDto { };

    _postRepo.Setup(r => r.GetAllPosts(param)).ReturnsAsync(new GetAllDataDto<Post>
    {
      PageNumber = 1,
      TotalPages = 1,
      Total = 0,
      Data = []
    });

    // Act
    var result = await _service.GetAllPosts(param);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(0, result.Total);

    _postRepo.Verify(r => r.GetAllPosts(param), Times.Once);
  }

  [Fact]
  [Trait("Category", "PostService")]
  [Trait("Method", "GetAllPostsErrorDatabase")]
  public async Task GetAllPosts_InternalError()
  {
    var param = new PostQueryParamDto { };
    _postRepo.Setup(r => r.GetAllPosts(param)).ThrowsAsync(new Exception("db failure"));

    var exception = await Assert.ThrowsAsync<HttpException>(() => _service.GetAllPosts(param));

    // Assert
    Assert.Equal(500, exception.StatusCode);
  }

  #endregion

  #region UpdatePost Tests
  [Trait("Category", "PostService")]
  [Trait("Method", "UpdateOwnPost")]
  [Fact]
  public async Task UpdateOwnPostSuccessful()
  {
    var userId = 1;
    var samplePost = CreateSamplePost();
    var request = new UpdatePostRequest { Title = "title updated", Content = "Content updated", UserId = userId };
    _postRepo.Setup(r => r.GetByIdAsync(samplePost.Id)).ReturnsAsync(samplePost);
    _postRepo.Setup(r => r.UpdatePost(samplePost.Id, It.IsAny<Post>())).ReturnsAsync(samplePost);
    _postRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

    var result = await _service.UpdatePost(samplePost.Id, request);

    Assert.NotNull(result);
    Assert.Equal(samplePost.Title, result.Title);
    Assert.Equal(samplePost.Content, result.Content);
    _postRepo.Verify(r => r.GetByIdAsync(samplePost.UserId), Times.Once);
    _postRepo.Verify(r => r.UpdatePost(samplePost.Id, It.IsAny<Post>()), Times.Once);
  }

  [Trait("Category", "PostService")]
  [Trait("Method", "UpdateNonExistingPost")]
  [Fact]
  public async Task UpdateNonExistingPost()
  {
    var request = new UpdatePostRequest { Title = "title updated", Content = "Content updated", UserId = 1 };
    _postRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Post?)null);

    var exception = await Assert.ThrowsAsync<HttpException>(() => _service.UpdatePost(999, request));

    Assert.Equal(404, exception.StatusCode);
    Assert.Equal("post not found", exception.Message);
  }

  [Trait("Category", "PostService")]
  [Trait("Method", "UpdateNotExistingCategory")]
  [Fact]
  public async Task UpdateNotExistingCategory()
  {
    var post = CreateSamplePost();
    var request = new UpdatePostRequest { Title = "title updated", Content = "Content updated", UserId = 1, CategoryId = 2 };
    _postRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);
    _postRepo.Setup(r => r.IsCategoryIdExist(2)).ReturnsAsync(false);

    var exception = await Assert.ThrowsAsync<HttpException>(() => _service.UpdatePost(1, request));

    Assert.Equal(404, exception.StatusCode);
    Assert.Equal("Category not found", exception.Message);
  }

  [Trait("Category", "PostService")]
  [Trait("Method", "UpdateOtherPostDetail")]
  [Fact]
  public async Task UpdateOtherPostDetail()
  {
    var userId = 2;
    var samplePost = CreateSamplePost();
    var request = new UpdatePostRequest { Title = "title updated", Content = "Content updated", UserId = userId };
    _postRepo.Setup(r => r.GetByIdAsync(samplePost.Id)).ReturnsAsync(samplePost);
    _postRepo.Setup(r => r.UpdatePost(samplePost.Id, It.IsAny<Post>())).ReturnsAsync(samplePost);
    var exception = await Assert.ThrowsAsync<HttpException>(() => _service.UpdatePost(samplePost.Id, request));

    Assert.Equal(403, exception.StatusCode);
    Assert.Equal("you do not have access here", exception.Message);
  }

  #endregion

  #region
  [Trait("Category", "PostService")]
  [Trait("Method", "PublishPost")]
  [Fact]
  public async Task PublishPostSuccessful()
  {
    var samplePost = CreateSamplePost();
    _postRepo.Setup(r => r.GetByIdAsync(samplePost.Id)).ReturnsAsync(samplePost);

    var result = await _service.UpdatePostStatus(samplePost.Id, true, 1);

    Assert.NotNull(result);
    Assert.Equal(true, result);
    _postRepo.Verify(r => r.GetByIdAsync(samplePost.UserId), Times.Once);
    _postRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
  }

  [Trait("Category", "PostService")]
  [Trait("Method", "UnpublishPost")]
  [Fact]
  public async Task UnpublishPostSuccessful()
  {
    var samplePost = CreateSamplePost();
    _postRepo.Setup(r => r.GetByIdAsync(samplePost.Id)).ReturnsAsync(samplePost);

    var result = await _service.UpdatePostStatus(samplePost.Id, false, 1);

    Assert.NotNull(result);
    Assert.Equal(false, result);
    _postRepo.Verify(r => r.GetByIdAsync(samplePost.UserId), Times.Once);
    _postRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
  }

  #endregion

}