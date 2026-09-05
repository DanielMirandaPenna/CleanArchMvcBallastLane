using CleanArchMvcBallastLane.Application.AssignmentTasks.GetById;
using CleanArchMvcBallastLane.Domain.Entities;
using CleanArchMvcBallastLane.Infra.Data.Repositories;
using FluentAssertions;

namespace CleanArchMvcBallastLane.Application.Tests;

public sealed class GetAssignmentsTaskByIdQueryHandlerTests(PostgreSqlFixture fixture) : IClassFixture<PostgreSqlFixture>, IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => fixture.ResetAsync();

    [Fact]
    public async Task Should_ReturnTaskById_When_GetByIdQueryIsHandled()
    {
        await using var context = fixture.CreateContext();
        var repository = new AssignmentTaskRepository(context);
        var expected = await repository.CreateAsync(new AssignmentTask("Find task", "Find description", "user-1"));
        var handler = new GetAssignmentsTaskByIdQueryHandler(repository);

        var result = await handler.Handle(new GetAssignmentTaskByIdQuery(expected.Id), CancellationToken.None);

        result.Should().BeEquivalentTo(expected, options => options.Excluding(task => task.CreatedAt));
    }

    [Fact]
    public async Task Should_ThrowApplicationException_When_TaskDoesNotExist()
    {
        await using var context = fixture.CreateContext();
        var handler = new GetAssignmentsTaskByIdQueryHandler(new AssignmentTaskRepository(context));

        var action = () => handler.Handle(new GetAssignmentTaskByIdQuery(999999), CancellationToken.None);

        await action.Should().ThrowAsync<KeyNotFoundException>().WithMessage("AssignmentTask with ID '999999' was not found.");
    }
}
