using CleanArchMvcBallastLane.Application.AssignmentTasks.Update;
using CleanArchMvcBallastLane.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CleanArchMvcBallastLane.API.Models.Requests;
public record AssignmentTaskUpdateRequest
{
    [Required(ErrorMessage = "The title is Required")]
    [MinLength(3)]
    [MaxLength(100)]
    public string Title { get; init; }

    [Required(ErrorMessage = "The description is Required")]
    [MinLength(3)]
    [MaxLength(200)]
    public string Description { get; init; }

    public Status Status { get; init; }

    [JsonIgnore]
    public string? CreatedBy { get; set; }

    public AssignmentTaskUpdateCommand ToCommand(int id)
    {
        return new AssignmentTaskUpdateCommand()
        {
            Id = id,
            CreatedBy = CreatedBy,
            Status = Status,
            Description = Description,
            Title = Title
        };
    }
}
