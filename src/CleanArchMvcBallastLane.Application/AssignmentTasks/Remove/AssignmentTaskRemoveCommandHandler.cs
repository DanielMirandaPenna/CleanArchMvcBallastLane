using CleanArchMvcBallastLane.Domain.Entities;
using CleanArchMvcBallastLane.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchMvcBallastLane.Application.AssignmentTasks.Remove
{
    public class AssignmentTaskRemoveCommandHandler(IAssignmentTaskRepository assignmentTaskRepository)
        : IRequestHandler<AssignmentTaskRemoveCommand, AssignmentTask>
    {
        public async Task<AssignmentTask> Handle(AssignmentTaskRemoveCommand request, CancellationToken cancellationToken)
        {
            var assignmentTask = await assignmentTaskRepository.GetById(request.Id);

            if (assignmentTask == null)
                throw new KeyNotFoundException($"AssignmentTask with ID '{request.Id}' was not found.");

            return await assignmentTaskRepository.RemoveAsync(assignmentTask);
        }
    }
}
