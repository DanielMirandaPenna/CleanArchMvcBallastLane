using CleanArchMvcBallastLane.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchMvcBallastLane.Application.AssignmentTasks.GetByUserId
{
    public class GetAssignmentTaskByIdUserQuery : IRequest<IEnumerable<AssignmentTask>>
    {
        public string CreatedBy { get; init; }
        public GetAssignmentTaskByIdUserQuery(string userId)
        {
            CreatedBy = userId;
        }
    }
}
