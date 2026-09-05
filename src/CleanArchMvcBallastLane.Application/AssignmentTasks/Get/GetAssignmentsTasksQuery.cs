using CleanArchMvcBallastLane.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchMvcBallastLane.Application.AssignmentTasks.Get
{
    public class GetAssignmentsTasksQuery : IRequest<IEnumerable<AssignmentTask>>
    {
    }
}
