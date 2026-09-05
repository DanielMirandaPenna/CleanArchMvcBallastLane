using CleanArchMvcBallastLane.Domain.Entities;
using CleanArchMvcBallastLane.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchMvcBallastLane.Application.AssignmentTasks
{
    public abstract class AssignmentTaskCommand : IRequest<AssignmentTask>
    {
        public string Title { get; init; }
        public string Description { get; init; }
        public string CreatedBy { get; init; }
        public Status Status { get; init; }
    }
}
