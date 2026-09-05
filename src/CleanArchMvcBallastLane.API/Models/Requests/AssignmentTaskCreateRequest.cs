using CleanArchMvcBallastLane.Application.AssignmentTasks.Create;
using System.ComponentModel.DataAnnotations;

namespace CleanArchMvcBallastLane.API.Models.Requests;

public record AssignmentTaskCreateRequest
{
    [Required(ErrorMessage = "The title is Required")]
    [MinLength(3)]
    [MaxLength(100)]
    public string Title { get; init; }

    [Required(ErrorMessage = "The description is Required")]
    [MinLength(3)]
    [MaxLength(200)]
    public string Description { get; init; }

    public AssignmentTaskCreateCommand ToCommand(string createdBy)
    {
        return new AssignmentTaskCreateCommand()
        {
            CreatedBy = createdBy,
            Description = Description,
            Title = Title
        };
    }
}