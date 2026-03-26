using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ITaskListShareRepository
    {
        Task<IEnumerable<TaskListShare>> GetSharesForTaskListAsync(string taskListId);
        Task<bool> IsUserSharedAsync(string taskListId, string userId);
        Task AddShareAsync(TaskListShare share);
        Task RemoveShareAsync(string taskListId, string userId);
    }
}
