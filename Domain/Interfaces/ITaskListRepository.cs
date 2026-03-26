using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ITaskListRepository
    {
        Task<TaskList?> GetByIdAsync(string id);
        Task<IEnumerable<TaskList>> GetByOwnerIdAsync(string ownerId, int skip, int take);
        Task<IEnumerable<TaskList>> GetBySharedWithUserIdAsync(string userId, int skip, int take);
        Task<TaskList> CreateAsync(TaskList taskList);
        Task UpdateAsync(TaskList taskList);
        Task DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
    }
}
