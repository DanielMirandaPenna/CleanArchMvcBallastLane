using CleanArchMvcBallastLane.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchMvcBallastLane.Application.AssignmentTasks.Remove
{
    public class AssignmentTaskRemoveCommand : IRequest<AssignmentTask>
    {
        public int Id { get; init; }
        public AssignmentTaskRemoveCommand(int id)
        {
            Id = id;
        }
    }
}
