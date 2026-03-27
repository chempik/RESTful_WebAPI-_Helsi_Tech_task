using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using System.Linq.Dynamic.Core;

namespace Application.Services
{
    public class TaskListService : ITaskListService
    {
        private readonly ITaskListRepository _taskListRepo;
        private readonly ITaskListShareRepository _shareRepo;
        private readonly IMapper _mapper;

        public TaskListService(ITaskListRepository taskListRepo, ITaskListShareRepository shareRepo, IMapper mapper)
        {
            _taskListRepo = taskListRepo;
            _shareRepo = shareRepo;
            _mapper = mapper;
        }

        private async Task<TaskList> EnsureAccessAsync(string userId, string taskListId, bool requireOwner = false)
        {
            var taskList = await _taskListRepo.GetByIdAsync(taskListId);
            if (taskList == null)
                throw new Exception($"Task list with id {taskListId} not found.");

            if (requireOwner)
            {
                if (taskList.OwnerId != userId)
                    throw new UnauthorizedAccessException("Only the owner can perform this action.");
                return taskList;
            }

            // Check: owner or shared
            if (taskList.OwnerId != userId && !await _shareRepo.IsUserSharedAsync(taskListId, userId))
                throw new UnauthorizedAccessException("You do not have permission to access this task list.");

            return taskList;
        }

        public async Task<ResponseDto> CreateAsync(string currentUserId, CreateDto dto)
        {
            var entity = _mapper.Map<TaskList>(dto);
            entity.OwnerId = currentUserId;
            entity.Id = Guid.NewGuid().ToString();
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            var created = await _taskListRepo.CreateAsync(entity);
            return _mapper.Map<ResponseDto>(created);
        }

        public async Task<ResponseDto> UpdateAsync(string currentUserId, UpdateDto dto)
        {
            var taskList = await EnsureAccessAsync(currentUserId, dto.Id);
            _mapper.Map(dto, taskList); // update Name and UpdatedAt
            await _taskListRepo.UpdateAsync(taskList);
            return _mapper.Map<ResponseDto>(taskList);
        }

        public async Task DeleteAsync(string currentUserId, string taskListId)
        {
            // only owner can remove
            await EnsureAccessAsync(currentUserId, taskListId, requireOwner: true);
            await _taskListRepo.DeleteAsync(taskListId);
            // remove all relation
            await _shareRepo.RemoveAllForTaskListAsync(taskListId);
        }

        public async Task<ResponseDto> GetByIdAsync(string currentUserId, string taskListId)
        {
            await EnsureAccessAsync(currentUserId, taskListId);
            var taskList = await _taskListRepo.GetByIdAsync(taskListId);
            return _mapper.Map<ResponseDto>(taskList);
        }

        public async Task<PagedResult<SummaryDto>> GetUserListsAsync(string currentUserId, int page, int pageSize)
        {
            // get lists where user is owner or has access
            var owned = await _taskListRepo.GetByOwnerIdAsync(currentUserId, (page - 1) * pageSize, pageSize);
            var shared = await _taskListRepo.GetBySharedWithUserIdAsync(currentUserId, (page - 1) * pageSize, pageSize);

            // We combine, sort by creation time (from newest to oldest)
            var all = owned.Concat(shared)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalCount = owned.Count() + shared.Count(); 

            var items = _mapper.Map<IEnumerable<SummaryDto>>(all);
            return new PagedResult<SummaryDto>(items, totalCount, page, pageSize);
        }

        public async Task AddShareAsync(string currentUserId, string taskListId, string targetUserId)
        {
            // Checking access to the list (owner or shared)
            await EnsureAccessAsync(currentUserId, taskListId);

            // We do not add the owner himself or if there is already one
            var taskList = await _taskListRepo.GetByIdAsync(taskListId);
            if (taskList.OwnerId == targetUserId)
                throw new Exception("Cannot share with the owner.");

            if (await _shareRepo.IsUserSharedAsync(taskListId, targetUserId))
                throw new Exception("User already has access.");

            var share = new TaskListShare
            {
                Id = Guid.NewGuid().ToString(),
                TaskListId = taskListId,
                UserId = targetUserId
            };
            await _shareRepo.AddShareAsync(share);
        }

        public async Task<IEnumerable<string>> GetSharesAsync(string currentUserId, string taskListId)
        {
            await EnsureAccessAsync(currentUserId, taskListId);
            var shares = await _shareRepo.GetSharesForTaskListAsync(taskListId);
            return shares.Select(s => s.UserId);
        }

        public async Task RemoveShareAsync(string currentUserId, string taskListId, string targetUserId)
        {
            await EnsureAccessAsync(currentUserId, taskListId);
            await _shareRepo.RemoveShareAsync(taskListId, targetUserId);
        }
    }
}
