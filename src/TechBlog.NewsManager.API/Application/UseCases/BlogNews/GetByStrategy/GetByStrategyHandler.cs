using AutoMapper;
using FluentValidation;
using TechBlog.NewsManager.API.Domain.Extensions;
using TechBlog.NewsManager.API.Domain.Logger;
using TechBlog.NewsManager.API.Domain.Responses;
using TechBlog.NewsManager.API.Domain.Strategies.GetBlogNews;

namespace TechBlog.NewsManager.API.Application.UseCases.BlogNews.GetByStrategy
{
    public static class GetByStrategyHandler
    {
        public static string Route => "/api/v1/blognew";
        public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
        public static Delegate Handle => Action;

        /// <summary>
        /// Get blog news by a defined strategy
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        /// <param name="validator"></param>
        /// <param name="getBlogNewsStrategies"></param>
        /// <param name="strategy">The strategy that defines the business rules</param>
        /// <param name="id">The Id of the blog new (optional)</param>
        /// <param name="name">The Name of a blog new (optional)</param>
        /// <param name="tags">The tags of the blog new (optional)</param>
        /// <param name="from">The datetime from begin for the query (optional)</param>
        /// <param name="to">The end datetime for the query (optional)</param>
        /// <param name="cancellationToken"></param>
        /// <remarks>
        /// To use the endpoint, use one of the defined strategies:
        ///   - GET_BY_ID,
        ///   - GET_BY_NAME,
        ///   - GET_BY_CREATE_DATE,
        ///   - GET_BY_CREATE_OR_UPDATE_DATE,
        ///   - GET_BY_TAGS
        /// </remarks>
        /// <response code="200" cref="BaseResponseWithValue{T}">Success</response>
        /// <response code="400" cref="BaseResponse">Invalid information for the strategy</response>
        /// <response code="404" cref="BaseResponse">When searching a specified new but was not found</response>
        /// <returns></returns>
        public static async Task<IResult> Action(ILoggerManager logger,
                                                   IMapper mapper,
                                                   IValidator<GetByStrategyRequest> validator,
                                                   IEnumerable<IGetBlogNewsStrategy> getBlogNewsStrategies,
                                                   GetBlogNewsStrategy strategy,
                                                   Guid? id,
                                                   string name,
                                                   string[] tags,
                                                   DateTime? from,
                                                   DateTime? to,
                                                   CancellationToken cancellationToken)
        {
            var request = new GetByStrategyRequest(strategy, id, name, tags, from, to);

            validator.ThrowIfInvalid(request);

            logger.Log("Begin get news", LoggerManagerSeverity.DEBUG, ("strategy", request.Strategy));

            var response = new BaseResponseWithValue<object>();

            logger.Log("Searching strategy", LoggerManagerSeverity.DEBUG, ("strategy", Enum.GetName(strategy)));

            var matchedStrategies = getBlogNewsStrategies.Where(s => s.Strategy == strategy).ToList();

            if (matchedStrategies.Count == 0)
            {
                logger.LogException("The strategy is not implemented", LoggerManagerSeverity.CRITICAL, parameters: ("strategy", Enum.GetName(strategy)));

                throw new NotImplementedException("The strategy is not implemented");
            }

            if (matchedStrategies.Count > 1)
            {
                logger.LogException("The strategy has more than one implementation", LoggerManagerSeverity.CRITICAL, parameters: ("strategy", Enum.GetName(strategy)));

                throw new ArgumentException("The strategy has more than one implementation");
            }

            logger.Log("Strategy was found", LoggerManagerSeverity.DEBUG, ("strategy", Enum.GetName(strategy)));

            var strategyBody = mapper.Map<GetBlogNewsStrategyBody>(request);

            var strategyResponse = await matchedStrategies[0].RunAsync(strategyBody, cancellationToken);

            logger.Log("End get news", LoggerManagerSeverity.DEBUG, ("strategy", request.Strategy));

            return Results.Ok(response.AsSuccess(strategyResponse));
        }
    }
}
