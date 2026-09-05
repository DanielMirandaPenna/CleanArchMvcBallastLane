using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchMvcBallastLane.Application.AssignmentTasks.Update;
public class AssignmentTaskUpdateCommand : AssignmentTaskCommand
{
    public int Id { get; init; }
}