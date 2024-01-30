using FluentValidation;
using System.Net.Mail;
using TechBlog.NewsManager.API.Domain.Extensions;
using TechBlog.NewsManager.API.Domain.Responses;

namespace TechBlog.NewsManager.API.Application.UseCases.BlogUsers.Create
{
    public class CreateBlogUserValidator : AbstractValidator<CreateBlogUserRequest>
    {
        public CreateBlogUserValidator()
        {
            RuleFor(c => c.Email).Cascade(CascadeMode.Continue)
                                 .Custom((email, context) =>
                                 {
                                     if (string.IsNullOrWhiteSpace(email) || !MailAddress.TryCreate(email, out _))
                                         context.AddFailure(ResponseMessage.InvalidEmail.GetDescription());
                                 });

            RuleFor(c => c.Name).Cascade(CascadeMode.Continue)
                                .NotEmpty()
                                .WithMessage(ResponseMessage.InvalidName.GetDescription());

            RuleFor(c => c.Password).Cascade(CascadeMode.Stop)
                                    .NotEmpty()
                                    .WithMessage(ResponseMessage.InvalidPassword.GetDescription())
                                    .MinimumLength(6)
                                    .WithMessage(ResponseMessage.InvalidPassword.GetDescription());

            RuleFor(c => c.BlogUserType).Cascade(CascadeMode.Continue)
                                        .NotEmpty()
                                        .WithMessage(ResponseMessage.InvalidUserType.GetDescription());
        }
    }
}
