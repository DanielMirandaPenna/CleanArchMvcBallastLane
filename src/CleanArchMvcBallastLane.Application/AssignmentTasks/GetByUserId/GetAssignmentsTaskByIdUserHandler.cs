using CleanArchMvcBallastLane.Domain.Entities;
using CleanArchMvcBallastLane.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchMvcBallastLane.Application.AssignmentTasks.GetByUserId
{
    public class GetAssignmentsTaskByIdUserHandler : IRequestHandler<GetAssignmentTaskByIdUserQuery, IEnumerable<AssignmentTask>>
    {
        private readonly IAssignmentTaskRepository _assignmentTaskRepository;

        public GetAssignmentsTaskByIdUserHandler(IAssignmentTaskRepository productRepository)
        {
            _assignmentTaskRepository = productRepository;
        }

        public async Task<IEnumerable<AssignmentTask>> Handle(GetAssignmentTaskByIdUserQuery request, CancellationToken cancellationToken)
        {
            return await _assignmentTaskRepository.GetByUserId(request.CreatedBy);
        }
    }
}
