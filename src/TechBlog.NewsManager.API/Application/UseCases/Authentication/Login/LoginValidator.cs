using FluentValidation;
using TechBlog.NewsManager.API.Domain.Extensions;
using TechBlog.NewsManager.API.Domain.Responses;

namespace TechBlog.NewsManager.API.Application.UseCases.Authentication.Login
{
    public class LoginValidator : AbstractValidator<LoginRequest>
    {
        public LoginValidator()
        {
            RuleFor(l => l.Username).Cascade(CascadeMode.Continue)
                                    .NotEmpty()
                                    .WithMessage(ResponseMessage.InvalidEmail.GetDescription());

            RuleFor(l => l.Password).Cascade(CascadeMode.Continue)
                                    .NotEmpty()
                                    .WithMessage(ResponseMessage.InvalidPassword.GetDescription());
        }
    }
}
