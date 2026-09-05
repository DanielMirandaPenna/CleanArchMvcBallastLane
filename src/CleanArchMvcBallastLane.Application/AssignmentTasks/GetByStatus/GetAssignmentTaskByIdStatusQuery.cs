using CleanArchMvcBallastLane.Domain.Entities;
using CleanArchMvcBallastLane.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchMvcBallastLane.Application.AssignmentTasks.GetByStatus
{
    public class GetAssignmentTaskByIdStatusQuery(Status status) : IRequest<IEnumerable<AssignmentTask>>
    {
        public Status Status { get; init; } = status;
    }
}
