using CleanArchMvcBallastLane.Application.AssignmentTasks.Create;
using CleanArchMvcBallastLane.Domain.Enums;
using CleanArchMvcBallastLane.Infra.Data.Repositories;
using FluentAssertions;

namespace CleanArchMvcBallastLane.Application.Tests;

public sealed class AssignmentTaskCreateCommandHandlerTests(PostgreSqlFixture fixture) : IClassFixture<PostgreSqlFixture>, IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => fixture.ResetAsync();

    [Fact]
    public async Task Should_PersistTaskWithExpectedFields_When_CreateCommandIsHandled()
    {
        await using var context = fixture.CreateContext();
        var repository = new AssignmentTaskRepository(context);
        var handler = new AssignmentTaskCreateCommandHandler(repository);
        var command = new AssignmentTaskCreateCommand
        {
            Title = "Created task",
            Description = "Created description",
            CreatedBy = "user-1",
            Status = Status.Pending
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().BeEquivalentTo(new
        {
            Title = command.Title,
            Description = command.Description,
            CreatedBy = command.CreatedBy,
            Status = Status.Pending
        }, options => options.ExcludingMissingMembers());
        (await context.AssignmentTasks.FindAsync(result.Id)).Should().BeEquivalentTo(result,
            options => options.Excluding(task => task.CreatedAt));
    }
}
