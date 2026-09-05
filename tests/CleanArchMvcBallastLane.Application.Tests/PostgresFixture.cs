using CleanArchMvcBallastLane.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace CleanArchMvcBallastLane.Application.Tests;

public sealed class PostgreSqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("cleanarchmvcballast")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await using var context = CreateContext();
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync() => await _container.DisposeAsync();

    public ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(ConnectionString, options => options.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
            .Options;

        return new ApplicationDbContext(options);
    }

    public async Task ResetAsync()
    {
        await using var context = CreateContext();
        context.AssignmentTasks.RemoveRange(context.AssignmentTasks);
        await context.SaveChangesAsync();
    }
}
