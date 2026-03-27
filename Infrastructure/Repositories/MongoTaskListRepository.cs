using Domain.Entities;
using Domain.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Repositories
{
    public class MongoTaskListRepository : ITaskListRepository
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<TaskList> _collection;

        public MongoTaskListRepository(IMongoDatabase database)
        {
            _database = database;
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

        public async Task<IEnumerable<TaskList>> GetBySharedWithUserIdAsync(string userId, int skip, int take)
        {
            var sharesCollection = _database.GetCollection<TaskListShare>("TaskListShares");
            var pipeline = new[]
            {
                // Filter shares by user
                new BsonDocument("$match", new BsonDocument("UserId", userId)),
                // Lookup task lists
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "TaskLists" },
                    { "localField", "TaskListId" },
                    { "foreignField", "Id" },
                    { "as", "taskList" }
                }),
                // Unwind the taskList array
                new BsonDocument("$unwind", new BsonDocument("path", "$taskList")),
                // Replace root with taskList document
                new BsonDocument("$replaceRoot", new BsonDocument("newRoot", "$taskList")),
                // Pagination
                new BsonDocument("$skip", skip),
                new BsonDocument("$limit", take)
            };

            var result = await sharesCollection.Aggregate<TaskList>(pipeline).ToListAsync();
            return result;
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
