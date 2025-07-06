using BlogAPI.DTOs;
using BlogAPI.Repository;
using FluentValidation;

public class CreatePostValidator : AbstractValidator<CreatePostRequest>
{
  private readonly IPostRepository _postRepository;

  public CreatePostValidator(IPostRepository postRepository)
  {
    _postRepository = postRepository;

    RuleFor(x => x.Title)
      .NotEmpty().WithMessage("title must not be empty");
    RuleFor(x => x.Content)
    .NotEmpty().WithMessage("title must not be empty");
    RuleFor(x => x.UserId)
    .NotEmpty().WithMessage("UserId must not be empty")
    .GreaterThan(0).WithMessage("UserId must be greater than 0")
    .MustAsync(async (userid, cancellation) => await _postRepository.IsUserIdExist(userid))
    .WithMessage("user does not exist");
    RuleFor(x => x.CategoryId)
    .MustAsync(async (categoryId, cancellation) => !categoryId.HasValue || await _postRepository.IsCategoryIdExist(categoryId.Value))
    .WithMessage("category does not exist");
  }
}

public class UpdatePostValidator : AbstractValidator<UpdatePostRequest>
{
  private readonly IPostRepository _postRepository;

  public UpdatePostValidator(IPostRepository postRepository)
  {
    _postRepository = postRepository;

    RuleFor(x => x.Title)
      .NotEmpty().WithMessage("title must not be empty");
    RuleFor(x => x.Content)
    .NotEmpty().WithMessage("title must not be empty");
    RuleFor(x => x.UserId)
    .NotEmpty().WithMessage("UserId must not be empty")
    .GreaterThan(0).WithMessage("UserId must be greater than 0")
    .MustAsync(async (userid, cancellation) => await _postRepository.IsUserIdExist(userid))
    .WithMessage("user does not exist");
    RuleFor(x => x.CategoryId)
    .MustAsync(async (categoryId, cancellation) => !categoryId.HasValue || await _postRepository.IsCategoryIdExist(categoryId.Value))
    .WithMessage("category does not exist");
  }
}