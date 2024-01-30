using AutoMapper;
using TechBlog.NewsManager.API.Application.ViewModels;
using TechBlog.NewsManager.API.Domain.Database;
using TechBlog.NewsManager.API.Domain.Exceptions;
using TechBlog.NewsManager.API.Domain.Logger;
using TechBlog.NewsManager.API.Domain.Strategies.GetBlogNews;

namespace TechBlog.NewsManager.API.Application.UseCases.BlogNews.GetByStrategy.Strategies
{
    public class GetByIdStrategy : IGetBlogNewsStrategy
    {
        private readonly ILoggerManager _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetBlogNewsStrategy Strategy => GetBlogNewsStrategy.GET_BY_ID;

        public GetByIdStrategy(ILoggerManager logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<object> RunAsync(GetBlogNewsStrategyBody body, CancellationToken cancellationToken)
        {
            _logger.Log("Getting blognew by id", LoggerManagerSeverity.DEBUG, ("strategy", Strategy), ("body", body));

            if (body is null || !body.ValidId)
            {
                _logger.Log("Invalid body", LoggerManagerSeverity.INFORMATION, ("strategy", Strategy), ("body", body));

                throw new BusinessException("Invalid strategy body");
            }

            var blogNew = await _unitOfWork.BlogNew.GetByIdAsync(body.Id, cancellationToken);

            if (!blogNew.Enabled)
            {
                _logger.Log("Blog new don't exists", LoggerManagerSeverity.INFORMATION, ("strategy", Strategy), ("body", body), ("blogNew", blogNew));

                throw new BusinessException("Blog new doesn't exists");
            }

            _logger.Log("Blog new found", LoggerManagerSeverity.DEBUG, ("strategy", Strategy), ("body", body), ("blogNew", blogNew));

            return _mapper.Map<BlogNewViewModel>(blogNew);
        }
    }
}
