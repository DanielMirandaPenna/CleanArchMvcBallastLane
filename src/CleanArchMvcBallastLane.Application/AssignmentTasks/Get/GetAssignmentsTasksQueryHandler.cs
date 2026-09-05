using CleanArchMvcBallastLane.Application.Common;
using CleanArchMvcBallastLane.Domain.Entities;
using CleanArchMvcBallastLane.Domain.Interfaces;
using MediatR;

namespace CleanArchMvcBallastLane.Application.AssignmentTasks.Get
{
    public class GetAssignmentsTasksQueryHandler : IRequestHandler<GetAssignmentsTasksQuery, PagedResult<AssignmentTask>>
    {
        private readonly IAssignmentTaskRepository _assignmentTaskRepository;

        public GetAssignmentsTasksQueryHandler(IAssignmentTaskRepository productRepository)
        {
            _assignmentTaskRepository = productRepository;
        }

        public async Task<PagedResult<AssignmentTask>> Handle(GetAssignmentsTasksQuery request, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _assignmentTaskRepository.GetAssignmentTasksPaged(request.PageNumber, request.PageSize);

            return new PagedResult<AssignmentTask>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
