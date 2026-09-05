using CleanArchMvcBallastLane.Domain.Enums;
using CleanArchMvcBallastLane.Domain.Validation;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace CleanArchMvcBallastLane.Domain.Entities
{
    public sealed class AssignmentTask : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public Status Status { get; set; }
        public string CreatedBy { get; set; }

        public AssignmentTask(string title, string description, string createdBy)
        {
            ValidateDomain(title, description, Status.Pending);
            CreatedAt = DateTime.Now.ToUniversalTime();
            CreatedBy = createdBy;
            Description = description;
        }

        public void Update(int id, string title, string description, Status status)
        {
            DomainExceptionValidation.When(id < 0, "Invalid Id value.");
            ValidateDomain(title, description, status);
            Id = id;
        }


        public void ChangeState(TaskAction action)
        {
            this.Status = (this.Status, action) switch
            {
                (Status.Pending, TaskAction.Started) => Status.InProgress,
                (Status.InProgress, TaskAction.Finish) => Status.Completed,
                _ => this.Status
            };
        }

        private void ValidateDomain(string title, string description, Status status)
        {
            DomainExceptionValidation.When(string.IsNullOrEmpty(title), "Invalid Title. Title is required");
            DomainExceptionValidation.When(title.Length < 3, "Invalid Title. Too short");
            DomainExceptionValidation.When(title.Length > 100, "Invalid Title. Too long");

            DomainExceptionValidation.When(string.IsNullOrEmpty(description), "Invalid description. description is required");
            DomainExceptionValidation.When(description.Length < 3, "Invalid description. Too short");
            DomainExceptionValidation.When(description.Length > 200, "Invalid descriptionle. Too long");

            DomainExceptionValidation.When(!Enum.IsDefined(typeof(Status), status), "Invalid Status.");

            Title = title;
            Description = description;
            Status = status;

        }
    }
}
