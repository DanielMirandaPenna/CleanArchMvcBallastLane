using CleanArchMvcBallastLane.Domain.Entities;
using CleanArchMvcBallastLane.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchMvcBallastLane.Application.AssignmentTasks.GetByStatus
{
    public class GetAssignmentsTaskByIdStatusQueryHandler : IRequestHandler<GetAssignmentTaskByIdStatusQuery, IEnumerable<AssignmentTask>>
    {
        private readonly IAssignmentTaskRepository _assignmentTaskRepository;

        public GetAssignmentsTaskByIdStatusQueryHandler(IAssignmentTaskRepository productRepository)
        {
            _assignmentTaskRepository = productRepository;
        }
        public async Task<IEnumerable<AssignmentTask>> Handle(GetAssignmentTaskByIdStatusQuery request, CancellationToken cancellationToken)
        {
            return await _assignmentTaskRepository.GetByStatus(request.Status);
        }
    }
}
