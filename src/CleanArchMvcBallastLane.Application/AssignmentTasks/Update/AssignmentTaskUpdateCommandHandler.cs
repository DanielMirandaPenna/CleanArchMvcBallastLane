using CleanArchMvcBallastLane.Domain.Entities;
using CleanArchMvcBallastLane.Domain.Interfaces;
using CleanArchMvcBallastLane.Domain.Validation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchMvcBallastLane.Application.AssignmentTasks.Update
{
    public class AssignmentTaskUpdateCommandHandler : IRequestHandler<AssignmentTaskUpdateCommand, AssignmentTask>
    {
        private readonly IAssignmentTaskRepository _assignmentTaskRepository;
        public AssignmentTaskUpdateCommandHandler(IAssignmentTaskRepository assignmentTaskRepository)
        {
            _assignmentTaskRepository = assignmentTaskRepository;
        }

        public async Task<AssignmentTask> Handle(AssignmentTaskUpdateCommand request, CancellationToken cancellationToken)
        {
            var assignmentTask = await _assignmentTaskRepository.GetById(request.Id);

            if (assignmentTask == null)
                throw new ApplicationException($"Entity could not be found.");

            if (assignmentTask.CreatedBy != request.CreatedBy)
                throw new DomainExceptionValidation("Only the owner can edit");

            assignmentTask.Update(request.Id, request.Title, request.Description, request.Status);
            return await _assignmentTaskRepository.UpdateAsync(assignmentTask);

        }
    }
}
