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
            var assignmentTask = await _assignmentTaskRepository.GetById(request.Id);

            if (assignmentTask == null)
                throw new KeyNotFoundException($"AssignmentTask with ID '{request.Id}' was not found.");

            return assignmentTask;
        }
    }
}
