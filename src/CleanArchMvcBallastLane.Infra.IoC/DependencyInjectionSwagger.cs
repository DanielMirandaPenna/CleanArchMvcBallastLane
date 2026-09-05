using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;

namespace CleanArchMvcBallastLane.Infra.IoC;

public static class DependencyInjectionSwagger
{
    private const string BearerScheme = "Bearer";

    public static IServiceCollection AddInfrastructureSwagger(
        this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "CleanArchMvcBallastLane.API",
                Version = "v1"
            });

            options.AddSecurityDefinition(BearerScheme, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Informe somente o token JWT."
            });

            options.AddSecurityRequirement(document =>
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(
                        BearerScheme,
                        document)] = []
                });
        });

        return services;
    }
}