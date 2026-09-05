using CleanArchMvcBallastLane.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchMvcBallastLane.Application.AssignmentTasks.GetById
{
    public class GetAssignmentTaskByIdQuery : IRequest<AssignmentTask>
    {
        public int Id { get; init; }

        public GetAssignmentTaskByIdQuery(int id)
        {
            Id = id;
        }
    }
}
