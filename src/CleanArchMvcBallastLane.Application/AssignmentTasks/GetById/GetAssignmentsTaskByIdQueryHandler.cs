using CleanArchMvcBallastLane.Domain.Entities;
using CleanArchMvcBallastLane.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchMvcBallastLane.Application.AssignmentTasks.GetById
{
    public class GetAssignmentsTaskByIdQueryHandler : IRequestHandler<GetAssignmentTaskByIdQuery, AssignmentTask>
    {
        private readonly IAssignmentTaskRepository _assignmentTaskRepository;

        public GetAssignmentsTaskByIdQueryHandler(IAssignmentTaskRepository productRepository)
        {
            _assignmentTaskRepository = productRepository;
        }

        public async Task<AssignmentTask> Handle(GetAssignmentTaskByIdQuery request, CancellationToken cancellationToken)
        {
            return await _assignmentTaskRepository.GetById(request.Id);
        }
    }
}
