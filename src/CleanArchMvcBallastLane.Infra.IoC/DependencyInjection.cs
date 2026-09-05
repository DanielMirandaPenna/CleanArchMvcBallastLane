using CleanArchMvcBallastLane.Domain.Account;
using CleanArchMvcBallastLane.Domain.Interfaces;
using CleanArchMvcBallastLane.Infra.Data.Context;
using CleanArchMvcBallastLane.Infra.Data.Identy;
using CleanArchMvcBallastLane.Infra.Data.Repositories;
using MediatR.NotificationPublishers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchMvcBallastLane.Infra.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
             options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));


            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.AddScoped<IAssignmentTaskRepository, AssignmentTaskRepository>();

            services.AddScoped<IAuthenticateService, AuthenticateService>();

            var myhandlers = AppDomain.CurrentDomain.Load("CleanArchMvcBallastLane.Application");
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(myhandlers);
                cfg.NotificationPublisherType = typeof(TaskWhenAllPublisher);
                cfg.Lifetime = ServiceLifetime.Scoped;
            });

            return services;
        }
    }
}
