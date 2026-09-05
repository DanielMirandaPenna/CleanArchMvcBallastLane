using CleanArchMvcBallastLane.Application.AssignmentTasks.GetByStatus;
using CleanArchMvcBallastLane.Domain.Entities;
using CleanArchMvcBallastLane.Domain.Enums;
using CleanArchMvcBallastLane.Infra.Data.Repositories;
using FluentAssertions;

namespace CleanArchMvcBallastLane.Application.Tests;

public sealed class GetAssignmentsTaskByIdStatusHandlerTests(PostgreSqlFixture fixture) : IClassFixture<PostgreSqlFixture>, IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => fixture.ResetAsync();

    [Fact]
    public async Task Should_ReturnTasksForStatus_When_GetByStatusQueryIsHandled()
    {
        await using var context = fixture.CreateContext();
        var repository = new AssignmentTaskRepository(context);
        var pending = await repository.CreateAsync(new AssignmentTask("Pending task", "Pending description", "user-1"));
        pending.Update(pending.Id, pending.Title, pending.Description, Status.InProgress);
        await repository.UpdateAsync(pending);
        await repository.CreateAsync(new AssignmentTask("Other task", "Other description", "user-2"));
        var handler = new GetAssignmentsTaskByIdStatusQueryHandler(repository);

        var result = await handler.Handle(new GetAssignmentTaskByIdStatusQuery(Status.InProgress), CancellationToken.None);

        result.Select(task => new { task.Title, task.Status })
            .Should().BeEquivalentTo([new { Title = "Pending task", Status = Status.InProgress }]);
    }
}
