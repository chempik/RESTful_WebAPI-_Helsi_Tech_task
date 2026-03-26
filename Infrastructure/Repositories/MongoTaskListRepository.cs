using Domain.Entities;
using Domain.Interfaces;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class MongoTaskListRepository : ITaskListRepository
    {
        private readonly IMongoCollection<TaskList> _collection;

        public MongoTaskListRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<TaskList>("TaskLists");
        }

        public async Task<TaskList?> GetByIdAsync(string id)
        {
            return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TaskList>> GetByOwnerIdAsync(string ownerId, int skip, int take)
        {
            return await _collection.Find(x => x.OwnerId == ownerId)
                .Skip(skip).Limit(take).ToListAsync();
        }

        //TO DO: possibility of future expansion
        public async Task<IEnumerable<TaskList>> GetBySharedWithUserIdAsync(string userId, int skip, int take)
        {
            return Enumerable.Empty<TaskList>();
        }

        public async Task<TaskList> CreateAsync(TaskList taskList)
        {
            await _collection.InsertOneAsync(taskList);
            return taskList;
        }

        public async Task UpdateAsync(TaskList taskList)
        {
            await _collection.ReplaceOneAsync(x => x.Id == taskList.Id, taskList);
        }

        public async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(x => x.Id == id);
        }

        public async Task<bool> ExistsAsync(string id)
        {
            var count = await _collection.CountDocumentsAsync(x => x.Id == id);
            return count > 0;
        }
    }
}
