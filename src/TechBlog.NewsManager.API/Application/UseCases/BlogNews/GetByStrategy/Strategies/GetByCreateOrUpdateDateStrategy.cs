using AutoMapper;
using TechBlog.NewsManager.API.Application.ViewModels;
using TechBlog.NewsManager.API.Domain.Database;
using TechBlog.NewsManager.API.Domain.Exceptions;
using TechBlog.NewsManager.API.Domain.Logger;
using TechBlog.NewsManager.API.Domain.Strategies.GetBlogNews;

namespace TechBlog.NewsManager.API.Application.UseCases.BlogNews.GetByStrategy.Strategies
{
    public class GetByCreateOrUpdateDateStrategy : IGetBlogNewsStrategy
    {
        private readonly ILoggerManager _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetBlogNewsStrategy Strategy => GetBlogNewsStrategy.GET_BY_CREATE_OR_UPDATE_DATE;

        public GetByCreateOrUpdateDateStrategy(ILoggerManager logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<object> RunAsync(GetBlogNewsStrategyBody body, CancellationToken cancellationToken)
        {
            _logger.Log("Getting blognew by create or update date interval", LoggerManagerSeverity.DEBUG, ("strategy", Strategy), ("body", body));

            if (body is null || !body.ValidDateInterval)
            {
                _logger.Log("Invalid body", LoggerManagerSeverity.INFORMATION, ("strategy", Strategy), ("body", body));

                throw new BusinessException("Invalid strategy body");
            }

            var blogNews = await _unitOfWork.BlogNew.GetByCreateOrUpdateDateAsync(body.From, body.To, cancellationToken);

            _logger.Log("End getting blognew by create or update date interval", LoggerManagerSeverity.DEBUG, ("strategy", Strategy), ("body", body), ("newsFoundCount", blogNews.Count()));

            return _mapper.Map<IEnumerable<BlogNewViewModel>>(blogNews);
        }
    }
}
