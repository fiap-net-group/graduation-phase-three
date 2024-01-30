using FluentValidation;
using TechBlog.NewsManager.API.Domain.Extensions;
using TechBlog.NewsManager.API.Domain.Responses;

namespace TechBlog.NewsManager.API.Application.UseCases.BlogNews.Update
{
    public class UpdateBlogNewValidator : AbstractValidator<UpdateBlogNewRequest>
    {
        public UpdateBlogNewValidator()
        {
            RuleFor(c => c.Title).Cascade(CascadeMode.Continue)
                                 .NotEmpty()
                                 .WithMessage(ResponseMessage.InvalidTitle.GetDescription());

            RuleFor(c => c.Description).Cascade(CascadeMode.Continue)
                                        .NotEmpty()
                                        .WithMessage(ResponseMessage.InvalidDescription.GetDescription());

            RuleFor(c => c.Body).Cascade(CascadeMode.Continue)
                                .NotEmpty()
                                .WithMessage(ResponseMessage.InvalidBody.GetDescription());
        }
    }
}