namespace TechBlog.NewsManager.API.Infrastructure.Database
{
    public static class BlogNewsQueriesExtensions
    {
        public static string GetByNameQuery => @"
												SELECT	News.[Id], 
												 		News.[Title],	
												 		News.[Description],	
												 		News.[Body],	
												 		News.[Tags] AS [InternalTags],	
												 		News.[Enabled],
												 		News.[BlogUserId] AS [AuthorId],
														News.[CreatedAt],
														News.[LastUpdateAt],
														Users.[Id],
														Users.[Name],
														Users.[Email],
														Users.[BlogUserType],
														Users.[CreatedAt],
														Users.[LastUpdateAt]
												FROM	[BlogNew] News
												INNER
												JOIN	[AspNetUsers] Users
												ON		News.[BlogUserId] = Users.[Id]
												WHERE	LOWER(News.[Title]) LIKE '%' + LOWER(@name) + '%'";
        public static string GetByTagsQuery => @"SELECT	News.[Id], 
												 		News.[Title],	
												 		News.[Description],	
												 		News.[Body],	
												 		News.[Tags] AS [InternalTags],	
												 		News.[Enabled],
												 		News.[BlogUserId] AS [AuthorId],
												 		News.[CreatedAt],
												 		News.[LastUpdateAt],
												 		Users.[Id],
												 		Users.[Name],
												 		Users.[Email],
												 		Users.[BlogUserType],
												 		Users.[CreatedAt],
												 		Users.[LastUpdateAt]
												 FROM	[BlogNew] News
												 INNER
												 JOIN	[AspNetUsers] Users
												 ON		News.[BlogUserId] = Users.[Id]";
        public static string GetByCreateDateIntervalQuery => @"SELECT	News.[Id], 
												 						News.[Title],	
												 						News.[Description],	
												 						News.[Body],	
												 						News.[Tags] AS [InternalTags],	
												 						News.[Enabled],
												 						News.[BlogUserId] AS [AuthorId],
																		News.[CreatedAt],
																		News.[LastUpdateAt],
																		Users.[Id],
																		Users.[Name],
																		Users.[Email],
																		Users.[BlogUserType],
																		Users.[CreatedAt],
																		Users.[LastUpdateAt]
																FROM	[BlogNew] News
																INNER
																JOIN	[AspNetUsers] Users
																ON		News.[BlogUserId] = Users.[Id]
																WHERE	News.[CreatedAt] >= @from
																AND	News.[CreatedAt] <= @to";
        public static string GetByCreateOrUpdateDateIntervalQuery => @"SELECT	News.[Id], 
																				News.[Title],	
																				News.[Description],	
																				News.[Body],	
																				News.[Tags] AS [InternalTags],	
																				News.[Enabled],
																				News.[BlogUserId] AS [AuthorId],
																				News.[CreatedAt],
																				News.[LastUpdateAt],
																				Users.[Id],
																				Users.[Name],
																				Users.[Email],
																				Users.[BlogUserType],
																				Users.[CreatedAt],
																				Users.[LastUpdateAt]
																		FROM	[BlogNew] News
																		INNER
																		JOIN	[AspNetUsers] Users
																		ON		News.[BlogUserId] = Users.[Id]
																		WHERE	(News.[CreatedAt] >= @from
																				AND	News.[CreatedAt] <= @to)
																		OR		(News.[LastUpdateAt] >= @from
																				AND	News.[LastUpdateAt] <= @to)";
    }
}
