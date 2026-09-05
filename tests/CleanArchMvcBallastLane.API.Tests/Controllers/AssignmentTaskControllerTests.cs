using CleanArchMvcBallastLane.API.Controllers;
using CleanArchMvcBallastLane.API.Models.Requests;
using CleanArchMvcBallastLane.Application.AssignmentTasks.Create;
using CleanArchMvcBallastLane.Application.AssignmentTasks.Get;
using CleanArchMvcBallastLane.Application.AssignmentTasks.GetById;
using CleanArchMvcBallastLane.Application.AssignmentTasks.GetByStatus;
using CleanArchMvcBallastLane.Application.AssignmentTasks.GetByUserId;
using CleanArchMvcBallastLane.Application.AssignmentTasks.Remove;
using CleanArchMvcBallastLane.Application.AssignmentTasks.Update;
using CleanArchMvcBallastLane.Application.Common;
using CleanArchMvcBallastLane.Domain.Entities;
using CleanArchMvcBallastLane.Domain.Enums;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace CleanArchMvcBallastLane.API.Tests.Controllers;
public class AssignmentTaskControllerTests
{
    private const string UserId = "user-123";

    private static AssignmentTaskController CreateController(Mock<IMediator> mediator)
    {
        var controller = new AssignmentTaskController(mediator.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity([
                        new Claim(ClaimTypes.NameIdentifier, UserId)
                    ], "Test"))
                }
            }
        };

        return controller;
    }

    [Fact]
    public async Task Should_ReturnOkWithMappedTasks_When_GetIsCalled()
    {
        var mediator = new Mock<IMediator>();
        var task = CreateTask(7);
        mediator.Setup(x => x.Send(It.IsAny<GetAssignmentsTasksQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<AssignmentTask>
            {
                Items = [task],
                TotalCount = 1,
                PageNumber = 1,
                PageSize = 10
            });
        var controller = CreateController(mediator);

        var result = await controller.Get();

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var paged = ok.Value.Should().BeOfType<CleanArchMvcBallastLane.API.Models.Responses.PagedResponse<CleanArchMvcBallastLane.API.Models.Responses.AssignmentTaskResponse>>().Subject;
        var response = paged.Items.Should().ContainSingle().Subject;
        response.Id.Should().Be(7);
        response.CreatedBy.Should().Be(UserId);
        mediator.Verify(x => x.Send(It.IsAny<GetAssignmentsTasksQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_SendRequestedIdAndReturnOk_When_GetByIdIsCalled()
    {
        var mediator = new Mock<IMediator>();
        var task = CreateTask(3);
        mediator.Setup(x => x.Send(It.Is<GetAssignmentTaskByIdQuery>(q => q.Id == 3), It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);
        var controller = CreateController(mediator);

        var result = await controller.Get(3);

        result.Result.Should().BeOfType<OkObjectResult>();
        mediator.Verify(x => x.Send(It.Is<GetAssignmentTaskByIdQuery>(q => q.Id == 3), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_SendRequestedUserAndReturnOk_When_GetByUserIsCalled()
    {
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.Is<GetAssignmentTaskByIdUserQuery>(q => q.CreatedBy == UserId), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        var controller = CreateController(mediator);

        var result = await controller.GetByUser(UserId);

        result.Result.Should().BeOfType<OkObjectResult>();
        mediator.Verify(x => x.Send(It.Is<GetAssignmentTaskByIdUserQuery>(q => q.CreatedBy == UserId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_SendRequestedStatusAndReturnOk_When_GetByStatusIsCalled()
    {
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.Is<GetAssignmentTaskByIdStatusQuery>(q => q.Status == Status.Completed), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        var controller = CreateController(mediator);

        var result = await controller.GetByStatus(Status.Completed);

        result.Result.Should().BeOfType<OkObjectResult>();
        mediator.Verify(x => x.Send(It.Is<GetAssignmentTaskByIdStatusQuery>(q => q.Status == Status.Completed), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_CreateTaskUsingAuthenticatedUser_When_PostIsCalled()
    {
        var mediator = new Mock<IMediator>();
        var task = CreateTask(11);
        mediator.Setup(x => x.Send(It.Is<AssignmentTaskCreateCommand>(c =>
                c.Title == "Task title" && c.Description == "Task description" && c.CreatedBy == UserId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);
        var controller = CreateController(mediator);

        var result = await controller.Post(new AssignmentTaskCreateRequest
        {
            Title = "Task title",
            Description = "Task description"
        });

        var created = result.Should().BeOfType<CreatedAtRouteResult>().Subject;
        created.RouteName.Should().Be("GetTask");
        created.RouteValues!["id"].Should().Be(11);
        mediator.Verify(x => x.Send(It.Is<AssignmentTaskCreateCommand>(c => c.CreatedBy == UserId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_SetAuthenticatedUserAndRequestedId_When_PutIsCalled()
    {
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<AssignmentTaskUpdateCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AssignmentTask("Task title", "Task description", UserId));
        var controller = CreateController(mediator);
        var request = new AssignmentTaskUpdateRequest
        {
            Title = "Task title",
            Description = "Task description",
            Status = Status.InProgress
        };

        var result = await controller.Put(9, request);

        result.Should().BeOfType<OkResult>();
        request.CreatedBy.Should().Be(UserId);
        mediator.Verify(x => x.Send(It.Is<AssignmentTaskUpdateCommand>(c =>
            c.Id == 9 && c.CreatedBy == UserId && c.Status == Status.InProgress), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_SendRemoveCommandAndReturnOk_When_DeleteIsCalled()
    {
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.Is<AssignmentTaskRemoveCommand>(c => c.Id == 5), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AssignmentTask("Task title", "Task description", UserId));
        var controller = CreateController(mediator);

        var result = await controller.Delete(5);

        result.Result.Should().BeOfType<OkResult>();
        mediator.Verify(x => x.Send(It.Is<AssignmentTaskRemoveCommand>(c => c.Id == 5), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_PropagateApplicationException_When_GetByIdTaskDoesNotExist()
    {
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.Is<GetAssignmentTaskByIdQuery>(q => q.Id == 999), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ApplicationException("Entity could not be found."));
        var controller = CreateController(mediator);

        var action = () => controller.Get(999);

        await action.Should().ThrowAsync<ApplicationException>().WithMessage("Entity could not be found.");
    }

    private static AssignmentTask CreateTask(int id)
    {
        var task = new AssignmentTask("Task title", "Task description", UserId);
        typeof(AssignmentTask).BaseType!.GetProperty(nameof(AssignmentTask.Id))!.SetValue(task, id);
        return task;
    }
}