using CleanArchMvcBallastLane.Domain.Entities;
using CleanArchMvcBallastLane.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchMvcBallastLane.Domain.Interfaces
{
    public interface IAssignmentTaskRepository
    {
        Task<AssignmentTask> CreateAsync(AssignmentTask task);
        Task<AssignmentTask> UpdateAsync(AssignmentTask task);
        Task<AssignmentTask> RemoveAsync(AssignmentTask task);
        Task<AssignmentTask> GetById(int id);
        Task<IEnumerable<AssignmentTask>> GetByUserId(string userId);
        Task<IEnumerable<AssignmentTask>> GetByStatus(Status status);
        Task<IEnumerable<AssignmentTask>> GetAssignmentTasks();
        Task<(IEnumerable<AssignmentTask> Items, int TotalCount)> GetAssignmentTasksPaged(int pageNumber, int pageSize);
    }
}
