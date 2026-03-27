using Domain.Entities;
using Domain.Interfaces;
using MongoDB.Driver;

namespace Infrastructure.Repositories
{
    public class MongoTaskListShareRepository : ITaskListShareRepository
    {
        private readonly IMongoCollection<TaskListShare> _collection;

        public MongoTaskListShareRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<TaskListShare>("TaskListShares");
        }

        public async Task<IEnumerable<TaskListShare>> GetSharesForTaskListAsync(string taskListId)
        {
            return await _collection.Find(x => x.TaskListId == taskListId).ToListAsync();
        }

        public async Task<bool> IsUserSharedAsync(string taskListId, string userId)
        {
            var count = await _collection.CountDocumentsAsync(x => x.TaskListId == taskListId && x.UserId == userId);
            return count > 0;
        }

        public async Task AddShareAsync(TaskListShare share)
        {
            await _collection.InsertOneAsync(share);
        }

        public async Task RemoveShareAsync(string taskListId, string userId)
        {
            await _collection.DeleteOneAsync(x => x.TaskListId == taskListId && x.UserId == userId);
        }

        public async Task RemoveAllForTaskListAsync(string taskListId)
        {
            await _collection.DeleteManyAsync(x => x.TaskListId == taskListId);
        }
    }
}
