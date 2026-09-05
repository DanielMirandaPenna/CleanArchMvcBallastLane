using CleanArchMvcBallastLane.Application.AssignmentTasks.Remove;
using CleanArchMvcBallastLane.Domain.Entities;
using CleanArchMvcBallastLane.Infra.Data.Repositories;
using FluentAssertions;

namespace CleanArchMvcBallastLane.Application.Tests;

public sealed class AssignmentTaskRemoveCommandHandlerTests(PostgreSqlFixture fixture) : IClassFixture<PostgreSqlFixture>, IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => fixture.ResetAsync();

    [Fact]
    public async Task Should_RemoveTaskFromDatabase_When_RemoveCommandIsHandled()
    {
        await using var seedContext = fixture.CreateContext();
        var seedRepository = new AssignmentTaskRepository(seedContext);
        var existing = await seedRepository.CreateAsync(new AssignmentTask("Remove task", "Remove description", "user-1"));
        await using var handlerContext = fixture.CreateContext();
        var repository = new AssignmentTaskRepository(handlerContext);
        var handler = new AssignmentTaskRemoveCommandHandler(repository);

        var result = await handler.Handle(new AssignmentTaskRemoveCommand(existing.Id), CancellationToken.None);

        result.Should().BeEquivalentTo(existing, options => options.Excluding(task => task.CreatedAt));
        (await repository.GetById(existing.Id)).Should().BeNull();
    }

    [Fact]
    public async Task Should_ThrowApplicationException_When_RemovingMissingTask()
    {
        await using var context = fixture.CreateContext();
        var handler = new AssignmentTaskRemoveCommandHandler(new AssignmentTaskRepository(context));

        var action = () => handler.Handle(new AssignmentTaskRemoveCommand(999999), CancellationToken.None);

        await action.Should().ThrowAsync<ApplicationException>().WithMessage("Entity could not be found.");
    }
}
