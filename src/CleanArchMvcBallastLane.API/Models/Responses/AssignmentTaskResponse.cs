using CleanArchMvcBallastLane.Domain.Entities;
using CleanArchMvcBallastLane.Domain.Enums;

namespace CleanArchMvcBallastLane.API.Models.Responses;
public class AssignmentTaskResponse
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public Status Status { get; set; }

    public string? CreatedBy { get; set; }

    public static AssignmentTaskResponse ToResponse(AssignmentTask task)
    {
        return new AssignmentTaskResponse
        {
            CreatedBy = task.CreatedBy,
            Id = task.Id,
            Status = task.Status,
            Description = task.Description,
            Title = task.Description
        };
    }
}