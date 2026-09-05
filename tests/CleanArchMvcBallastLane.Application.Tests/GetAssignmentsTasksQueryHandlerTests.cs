using CleanArchMvcBallastLane.Application.AssignmentTasks.Get;
using CleanArchMvcBallastLane.Domain.Entities;
using CleanArchMvcBallastLane.Infra.Data.Repositories;
using FluentAssertions;

namespace CleanArchMvcBallastLane.Application.Tests;

public sealed class GetAssignmentsTasksQueryHandlerTests(PostgreSqlFixture fixture) : IClassFixture<PostgreSqlFixture>, IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => fixture.ResetAsync();

    [Fact]
    public async Task Should_ReturnAllPersistedTasks_When_GetQueryIsHandled()
    {
        await using var context = fixture.CreateContext();
        var repository = new AssignmentTaskRepository(context);
        var first = await repository.CreateAsync(new AssignmentTask("First task", "First description", "user-1"));
        var second = await repository.CreateAsync(new AssignmentTask("Second task", "Second description", "user-2"));
        var handler = new GetAssignmentsTasksQueryHandler(repository);

        var result = await handler.Handle(new GetAssignmentsTasksQuery(), CancellationToken.None);

        result.Items.Select(task => new { task.Title, task.Description, task.CreatedBy, task.Status })
            .Should().BeEquivalentTo([
                new { first.Title, first.Description, first.CreatedBy, first.Status },
                new { second.Title, second.Description, second.CreatedBy, second.Status }
            ]);
    }
}
