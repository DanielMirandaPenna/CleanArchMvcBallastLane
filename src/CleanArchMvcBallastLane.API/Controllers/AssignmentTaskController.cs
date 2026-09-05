using CleanArchMvcBallastLane.API.Models.Requests;
using CleanArchMvcBallastLane.API.Models.Responses;
using CleanArchMvcBallastLane.Application.AssignmentTasks.Get;
using CleanArchMvcBallastLane.Application.AssignmentTasks.GetById;
using CleanArchMvcBallastLane.Application.AssignmentTasks.GetByStatus;
using CleanArchMvcBallastLane.Application.AssignmentTasks.GetByUserId;
using CleanArchMvcBallastLane.Application.AssignmentTasks.Remove;
using CleanArchMvcBallastLane.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CleanArchMvcBallastLane.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AssignmentTaskController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AssignmentTaskController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResponse<AssignmentTaskResponse>>> Get(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetAssignmentsTasksQuery { PageNumber = pageNumber, PageSize = pageSize };
            var result = await _mediator.Send(query);

            return Ok(new PagedResponse<AssignmentTaskResponse>
            {
                Items = result.Items.Select(AssignmentTaskResponse.ToResponse),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages
            });
        }


        [HttpGet("{id}", Name = "GetTask")]
        public async Task<ActionResult<AssignmentTaskResponse>> Get(int id)
        {
            var query = new GetAssignmentTaskByIdQuery(id);
            var result = await _mediator.Send(query);
            return Ok(AssignmentTaskResponse.ToResponse(result));
        }

        [HttpGet()]
        [Route("{userId}/GetTaskByUser")]
        public async Task<ActionResult<AssignmentTaskResponse>> GetByUser(string userId)
        {
            var query = new GetAssignmentTaskByIdUserQuery(userId);
            var result = await _mediator.Send(query);
            return Ok(result.Select(AssignmentTaskResponse.ToResponse));
        }

        [HttpGet()]
        [Route("GetTaskByStatus")]
        public async Task<ActionResult<AssignmentTaskResponse>> GetByStatus([FromQuery] Status status)
        {
            var query = new GetAssignmentTaskByIdStatusQuery(status);
            var result = await _mediator.Send(query);
            return Ok(result.Select(AssignmentTaskResponse.ToResponse));
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AssignmentTaskCreateRequest assignmentTask)
        {
            var claimValue = User.Claims.FirstOrDefault(claim => claim.Type.Contains(ClaimTypes.NameIdentifier));

            var command = assignmentTask.ToCommand(claimValue.Value);
            var result = await _mediator.Send(command);
            return new CreatedAtRouteResult("GetTask", new { id = result.Id }, AssignmentTaskResponse.ToResponse(result));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] AssignmentTaskUpdateRequest assignmentTask)
        {
            var claimValue = User.Claims.FirstOrDefault(claim => claim.Type.Contains(ClaimTypes.NameIdentifier));
            assignmentTask.CreatedBy = claimValue.Value;

            var command = assignmentTask.ToCommand(id);
            await _mediator.Send(command);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<AssignmentTaskResponse>> Delete(int id)
        {
            var assignmentTaskRemoveCommand = new AssignmentTaskRemoveCommand(id);
            await _mediator.Send(assignmentTaskRemoveCommand);

            return Ok();
        }
    }
}