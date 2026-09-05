using CleanArchMvcBallastLane.Domain.Entities;
using CleanArchMvcBallastLane.Domain.Enums;
using CleanArchMvcBallastLane.Domain.Interfaces;
using CleanArchMvcBallastLane.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchMvcBallastLane.Infra.Data.Repositories
{
    public class AssignmentTaskRepository : IAssignmentTaskRepository
    {
        private readonly ApplicationDbContext _context;

        public AssignmentTaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AssignmentTask> CreateAsync(AssignmentTask assignmentTask)
        {
            _context.Add(assignmentTask);
            await _context.SaveChangesAsync();
            return assignmentTask;
        }

        public async Task<IEnumerable<AssignmentTask>> GetAssignmentTasks()
        {
            return await _context.AssignmentTasks.AsNoTracking().ToListAsync();
        }

        public async Task<AssignmentTask> GetById(int id)
        {
            return await _context.AssignmentTasks.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<AssignmentTask>> GetByStatus(Status status)
        {
            return await _context.AssignmentTasks.AsNoTracking().Where(p => p.Status == status).ToListAsync();
        }

        public async Task<IEnumerable<AssignmentTask>> GetByUserId(string userId)
        {
            return await _context.AssignmentTasks.AsNoTracking().Where(p => p.CreatedBy == userId).ToListAsync();
        }

        public async Task<AssignmentTask> RemoveAsync(AssignmentTask assignmentTask)
        {
            _context.Remove(assignmentTask);
            await _context.SaveChangesAsync();
            return assignmentTask;
        }

        public async Task<AssignmentTask> UpdateAsync(AssignmentTask assignmentTask)
        {
            _context.Update(assignmentTask);
            await _context.SaveChangesAsync();
            return assignmentTask;

        }
    }
}
