using CleanArchMvcBallastLane.Application.AssignmentTasks.Update;
using CleanArchMvcBallastLane.Domain.Entities;
using CleanArchMvcBallastLane.Domain.Enums;
using CleanArchMvcBallastLane.Domain.Validation;
using CleanArchMvcBallastLane.Infra.Data.Repositories;
using FluentAssertions;

namespace CleanArchMvcBallastLane.Application.Tests;

public sealed class AssignmentTaskUpdateCommandHandlerTests(PostgreSqlFixture fixture) : IClassFixture<PostgreSqlFixture>, IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => fixture.ResetAsync();

    [Fact]
    public async Task Should_UpdateOwnedTask_When_UpdateCommandIsHandled()
    {
        await using var seedContext = fixture.CreateContext();
        var seedRepository = new AssignmentTaskRepository(seedContext);
        var existing = await seedRepository.CreateAsync(new AssignmentTask("Old title", "Old description", "user-1"));
        await using var handlerContext = fixture.CreateContext();
        var handler = new AssignmentTaskUpdateCommandHandler(new AssignmentTaskRepository(handlerContext));
        var command = new AssignmentTaskUpdateCommand
        {
            Id = existing.Id,
            Title = "New title",
            Description = "New description",
            CreatedBy = "user-1",
            Status = Status.Completed
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().BeEquivalentTo(new
        {
            Id = existing.Id,
            Title = command.Title,
            Description = command.Description,
            CreatedBy = command.CreatedBy,
            Status = command.Status
        }, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task Should_ThrowDomainException_When_NonOwnerUpdatesTask()
    {
        await using var context = fixture.CreateContext();
        var repository = new AssignmentTaskRepository(context);
        var existing = await repository.CreateAsync(new AssignmentTask("Owned title", "Owned description", "owner"));
        var handler = new AssignmentTaskUpdateCommandHandler(repository);
        var command = new AssignmentTaskUpdateCommand
        {
            Id = existing.Id,
            Title = "New title",
            Description = "New description",
            CreatedBy = "other-user",
            Status = Status.Completed
        };

        var action = () => handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<DomainExceptionValidation>().WithMessage("Only the owner can edit");
    }

    [Fact]
    public async Task Should_ThrowApplicationException_When_UpdatingMissingTask()
    {
        await using var context = fixture.CreateContext();
        var handler = new AssignmentTaskUpdateCommandHandler(new AssignmentTaskRepository(context));
        var command = new AssignmentTaskUpdateCommand
        {
            Id = 999999,
            Title = "New title",
            Description = "New description",
            CreatedBy = "user-1",
            Status = Status.Completed
        };

        var action = () => handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<ApplicationException>().WithMessage("Entity could not be found.");
    }
}
