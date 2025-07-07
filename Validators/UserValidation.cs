namespace BlogAPI.Validators;

using BlogAPI.DTOs;
using BlogAPI.Repository;
using FluentValidation;

public class LoginValidator : AbstractValidator<LoginRequest>
{

  public LoginValidator()
  {

    RuleFor(x => x.Password)
      .NotEmpty().WithMessage("Password must not be empty");

    RuleFor(x => x.Email)
      .NotEmpty().WithMessage("email must not be empty")
      .EmailAddress().WithMessage("invalid email format");
  }
}

public class CreateUserValidator : AbstractValidator<CreateUserRequest>
{
  private readonly IUserRepository _userRepository;

  public CreateUserValidator(IUserRepository userRepository)
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

public class UpdateUserProfileValidator : AbstractValidator<UpdateUserProfileRequest>
{

  public UpdateUserProfileValidator()
  {

    RuleFor(x => x.Firstname)
      .NotEmpty().WithMessage("username must not be empty")
      .MinimumLength(5).WithMessage("username is too short");
  }
}
