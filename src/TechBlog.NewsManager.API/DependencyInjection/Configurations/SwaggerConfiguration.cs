using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace TechBlog.NewsManager.API.DependencyInjection.Configurations
{
    [ExcludeFromCodeCoverage]
    public static class SwaggerConfiguration
    {
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.OperationFilter<SwaggerDefaultValues>();

                options.OperationFilter<ApiKeyHeaderParameter>();

                options.CustomSchemaIds(SchemaIdStrategy);

                options.AddSecurity();

                options.IncludeCommentsToApiDocumentation();
            });

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            return services;
        }

        private static string SchemaIdStrategy(Type currentClass)
        {
            var builder = new StringBuilder(currentClass.Name.Replace("ViewModel", string.Empty).Replace("Model", string.Empty));

            for (int i = 0; i < currentClass.GenericTypeArguments.Length; i++)
                builder.Append(currentClass.GenericTypeArguments[0].Name);

            return builder.ToString();
        }

        private static void AddSecurity(this SwaggerGenOptions options)
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        }

        private static void IncludeCommentsToApiDocumentation(this SwaggerGenOptions options)
        {
            try
            {
                options.TryIncludeCommentsToApiDocumentation();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void TryIncludeCommentsToApiDocumentation(this SwaggerGenOptions options)
        {
            var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";

            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            options.IncludeXmlComments(xmlPath);
        }


        internal static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
        {
            app.UseSwagger();

            app.UseSwaggerUI();

            return app;
        }
    }
    [ExcludeFromCodeCoverage]
    internal sealed class SwaggerDefaultValues : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;

            foreach (var responseType in context.ApiDescription.SupportedResponseTypes)
            {
                var responseKey = responseType.IsDefaultResponse ? "default" : responseType.StatusCode.ToString();
                var response = operation.Responses[responseKey];

                foreach (var contentType in from contentType in response.Content.Keys
                                            where responseType.ApiResponseFormats.All(x => x.MediaType != contentType)
                                            select contentType)
                {
                    response.Content.Remove(contentType);
                }
            }

            if (operation.Parameters == null)
                return;

            foreach (var parameter in operation.Parameters)
            {
                var description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);

                parameter.Description ??= description.ModelMetadata.Description;

                parameter.Required |= description.IsRequired;
            }
        }
    }

    [ExcludeFromCodeCoverage]
    internal sealed class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IConfiguration _configuration;

        public ConfigureSwaggerOptions(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(SwaggerGenOptions options)
        {
            options.SwaggerDoc("v1", CreateInfoForApiVersion(_configuration));
        }

        private static OpenApiInfo CreateInfoForApiVersion(IConfiguration configuration)
        {
            try
            {
                return TryCreateInfoForApiVersion(configuration);
            }
            catch (Exception)
            {
                return CreateInfoWithUndefinedInformations();
            }
        }

        private static OpenApiInfo TryCreateInfoForApiVersion(IConfiguration configuration)
        {
            if (!configuration.GetSection("Swagger").Exists())
            {
                return CreateInfoWithUndefinedInformations();
            }

            return CreateInfoWithDefinedInformations(configuration);
        }

        private static OpenApiInfo CreateInfoWithUndefinedInformations()
        {
            var info = new OpenApiInfo()
            {
                Title = Assembly.GetEntryAssembly().GetName().Name
            };

            return info;
        }

        static OpenApiInfo CreateInfoWithDefinedInformations(IConfiguration configuration)
        {
            var info = new OpenApiInfo()
            {
                Title = configuration.GetValue<string>("Swagger:Title"),
                Description = configuration.GetValue<string>("Swagger:Description"),
                Contact = new OpenApiContact()
                {
                    Name = configuration.GetValue<string>("Swagger:Contact:Name"),
                    Email = configuration.GetValue<string>("Swagger:Contact:Email")
                },
                License = new OpenApiLicense()
                {
                    Name = configuration.GetValue<string>("Swagger:License:Name"),
                    Url = new Uri(configuration.GetValue<string>("Swagger:License:Url"))
                }
            };

            return info;
        }
    }
    [ExcludeFromCodeCoverage]
    internal class ApiKeyHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-API-KEY",
                In = ParameterLocation.Header,
                Description = "Key to access the API",
                Required = false,
                Schema = default
            });
        }
    }
}
