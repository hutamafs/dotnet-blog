namespace BlogAPI.Validators;

using BlogAPI.DTOs;
using BlogAPI.Repository;
using FluentValidation;

public class CreateUpdateCommentValidator : AbstractValidator<CreateUpdateCommentRequest>
{
  private readonly ICommentRepository _commentRepository;
  private readonly IPostRepository _postRepository;

  public CreateUpdateCommentValidator(ICommentRepository commentRepository, IPostRepository postRepository)
  {
    _commentRepository = commentRepository;
    _postRepository = postRepository;

    RuleFor(x => x.Text)
      .NotEmpty().WithMessage("comment must not be empty");
    RuleFor(x => x.UserId)
    .NotEmpty().WithMessage("UserId must not be empty")
    .GreaterThan(0).WithMessage("UserId must be greater than 0")
    .MustAsync(async (userid, cancellation) => await _postRepository.IsUserIdExist(userid))
    .WithMessage("user does not exist");
    RuleFor(x => x.PostId)
    .MustAsync(async (PostId, cancellation) => await _commentRepository.IsPostIdExist(PostId))
    .WithMessage("post does not exist");
  }

}

public class UpdateCommentValidator : AbstractValidator<CreateUpdateCommentRequest>
{

  public UpdateCommentValidator()
  {

    RuleFor(x => x.Text)
      .NotEmpty().WithMessage("comment must not be empty");
  }

}
