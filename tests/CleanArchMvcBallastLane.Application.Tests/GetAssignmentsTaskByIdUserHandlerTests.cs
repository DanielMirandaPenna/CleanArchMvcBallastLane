using CleanArchMvcBallastLane.Application.AssignmentTasks.GetByUserId;
using CleanArchMvcBallastLane.Domain.Entities;
using CleanArchMvcBallastLane.Infra.Data.Repositories;
using FluentAssertions;

namespace CleanArchMvcBallastLane.Application.Tests;

public sealed class GetAssignmentsTaskByIdUserHandlerTests(PostgreSqlFixture fixture) : IClassFixture<PostgreSqlFixture>, IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => fixture.ResetAsync();

    [Fact]
    public async Task Should_ReturnTasksForUser_When_GetByUserQueryIsHandled()
    {
        await using var context = fixture.CreateContext();
        var repository = new AssignmentTaskRepository(context);
        await repository.CreateAsync(new AssignmentTask("Owned task", "Owned description", "user-1"));
        await repository.CreateAsync(new AssignmentTask("Other task", "Other description", "user-2"));
        var handler = new GetAssignmentsTaskByIdUserHandler(repository);

        var result = await handler.Handle(new GetAssignmentTaskByIdUserQuery("user-1"), CancellationToken.None);

        result.Select(task => new { task.Title, task.CreatedBy })
            .Should().BeEquivalentTo([new { Title = "Owned task", CreatedBy = "user-1" }]);
    }
}
