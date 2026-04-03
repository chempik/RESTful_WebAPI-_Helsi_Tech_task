using Domain.Entities;
using Domain.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Repositories
{
    public class MongoTaskListRepository : ITaskListRepository
    {
        private readonly IMongoCollection<TaskList> _collection;

        public MongoTaskListRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<TaskList>("TaskLists");

            CreateIndexes();
        }

        private void CreateIndexes()
        {
            var indexKeys = Builders<TaskList>.IndexKeys.Ascending(x => x.SharedWithUserIds);
            var indexModel = new CreateIndexModel<TaskList>(indexKeys);

            _collection.Indexes.CreateOne(indexModel);
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

        public async Task<IEnumerable<TaskList>> GetBySharedWithUserIdAsync(string userId, int skip, int take)
        {
            return await _collection.Find(x => x.SharedWithUserIds.Contains(userId))
          .Skip(skip).Limit(take).ToListAsync();
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
        public async Task AddShareAsync(string taskListId, string userId)
        {
            var update = Builders<TaskList>.Update.AddToSet(x => x.SharedWithUserIds, userId);
            await _collection.UpdateOneAsync(x => x.Id == taskListId, update);
        }

        public async Task RemoveShareAsync(string taskListId, string userId)
        {
            var update = Builders<TaskList>.Update.Pull(x => x.SharedWithUserIds, userId);
            await _collection.UpdateOneAsync(x => x.Id == taskListId, update);
        }

        public async Task<bool> IsUserSharedAsync(string taskListId, string userId)
        {
            var filter = Builders<TaskList>.Filter.Eq(x => x.Id, taskListId) &
                         Builders<TaskList>.Filter.AnyEq(x => x.SharedWithUserIds, userId);
            var count = await _collection.CountDocumentsAsync(filter);
            return count > 0;
        }

        public async Task<IEnumerable<string>> GetSharedUserIdsAsync(string taskListId)
        {
            var taskList = await GetByIdAsync(taskListId);
            return taskList?.SharedWithUserIds ?? Enumerable.Empty<string>();
        }
    }
}
