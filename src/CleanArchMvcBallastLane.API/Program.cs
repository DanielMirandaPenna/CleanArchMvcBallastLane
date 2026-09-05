using CleanArchMvcBallastLane.API.Models.Commons;
using CleanArchMvcBallastLane.API.Middlewares;
using CleanArchMvcBallastLane.Infra.Data.Context;
using CleanArchMvcBallastLane.Infra.IoC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddInfrastructureJWT(builder.Configuration);
builder.Services.AddInfrastructureSwagger();

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressMapClientErrors = true;
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = ErrorResponse.FromContext(context.HttpContext)
                .WithModelState(context.ModelState);

            return new BadRequestObjectResult(errors)
            {
                ContentTypes =
            {
                "application/problem+json"
            }
            };
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ExceptionHandlerMiddleware>();

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseQueryStrings = true;
    options.LowercaseUrls = true;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlerMiddleware>()
    .UseHttpsRedirection()
    .UseRouting()
    .UseCors("AllowAngular")
    .UseAuthentication()
    .UseAuthorization();

app.MapControllers();

try
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();
    await app.RunAsync();

}
catch (Exception e)
{
    Console.WriteLine(e);
}