using BlogAPI.DTOs;
using BlogAPI.Repository;
using FluentValidation;

public class PostValidator : AbstractValidator<CreateUpdatePostRequest>
{
  private readonly IPostRepository _postRepository;

  public PostValidator(IPostRepository postRepository)
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