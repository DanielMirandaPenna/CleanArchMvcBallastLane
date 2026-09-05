using CleanArchMvcBallastLane.Domain.Entities;
using CleanArchMvcBallastLane.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchMvcBallastLane.Application.AssignmentTasks.Get
{
    public class GetAssignmentsTasksQueryHandler : IRequestHandler<GetAssignmentsTasksQuery, IEnumerable<AssignmentTask>>
    {
        private readonly IAssignmentTaskRepository _assignmentTaskRepository;

        public GetAssignmentsTasksQueryHandler(IAssignmentTaskRepository productRepository)
        {
            _assignmentTaskRepository = productRepository;
        }

        public async Task<IEnumerable<AssignmentTask>> Handle(GetAssignmentsTasksQuery request, CancellationToken cancellationToken)
        {
            return await _assignmentTaskRepository.GetAssignmentTasks();
        }
    }
}
