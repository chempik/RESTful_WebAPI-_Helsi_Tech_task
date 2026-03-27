using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ITaskListShareRepository
    {
        Task<IEnumerable<TaskListShare>> GetSharesForTaskListAsync(string taskListId);
        Task<bool> IsUserSharedAsync(string taskListId, string userId);
        Task AddShareAsync(TaskListShare share);
        Task RemoveShareAsync(string taskListId, string userId);
        Task RemoveAllForTaskListAsync(string taskListId);
    }
}
