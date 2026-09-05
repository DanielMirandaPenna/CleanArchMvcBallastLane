using CleanArchMvcBallastLane.Application.Common;
using CleanArchMvcBallastLane.Domain.Entities;
using MediatR;

namespace CleanArchMvcBallastLane.Application.AssignmentTasks.Get
{
    public class GetAssignmentsTasksQuery : IRequest<PagedResult<AssignmentTask>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
