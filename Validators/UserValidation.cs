using BlogAPI.DTOs;
using BlogAPI.Repository;
using FluentValidation;

public class CreateUpdateValidator : AbstractValidator<CreateUpdateUserRequest>
{
  private readonly IUserRepository _userRepository;

  public CreateUpdateValidator(IUserRepository userRepository)
  {
    _userRepository = userRepository;

    RuleFor(x => x.Username)
      .NotEmpty().WithMessage("username must not be empty")
      .MinimumLength(5).WithMessage("username is too short")
      .MustAsync(async (username, cancellation) =>
        !await _userRepository.IsUsernameTakenAsync(username))
      .WithMessage("username already exists");

    RuleFor(x => x.Email)
      .NotEmpty().WithMessage("email must not be empty")
      .EmailAddress().WithMessage("invalid email format")
      .MustAsync(async (email, cancellation) =>
        !await _userRepository.IsEmailTakenAsync(email))
      .WithMessage("email already exists");
  }
}