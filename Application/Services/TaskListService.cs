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
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public TaskListService(ITaskListRepository taskListRepo, IMapper mapper, ICurrentUserService currentUserService)
        {
            _taskListRepo = taskListRepo;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        private async Task<TaskList> EnsureAccessAsync(string taskListId, bool requireOwner = false)
        {
            var userId = _currentUserService.GetCurrentUserId();
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
            if (taskList.OwnerId != userId && !taskList.SharedWithUserIds.Contains(userId))
                throw new UnauthorizedAccessException("You do not have permission to access this task list.");

            return taskList;
        }

        public async Task<ResponseDto> CreateAsync(CreateDto dto)
        {
            var userId = _currentUserService.GetCurrentUserId();

            var entity = _mapper.Map<TaskList>(dto);
            entity.OwnerId = userId;
            entity.Id = Guid.NewGuid().ToString();
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.SharedWithUserIds = new List<string>();

            var created = await _taskListRepo.CreateAsync(entity);
            return _mapper.Map<ResponseDto>(created);
        }

        public async Task<ResponseDto> UpdateAsync(UpdateDto dto)
        {
            if (string.IsNullOrEmpty(dto.Id))
                throw new Exception("Task list Id is required.");

            var taskList = await EnsureAccessAsync(dto.Id);
            _mapper.Map(dto, taskList);
            taskList.UpdatedAt = DateTime.UtcNow;
            await _taskListRepo.UpdateAsync(taskList);
            return _mapper.Map<ResponseDto>(taskList);
        }

        public async Task DeleteAsync(string taskListId)
        {
            // only owner can remove
            await EnsureAccessAsync(taskListId, requireOwner: true);
            await _taskListRepo.DeleteAsync(taskListId);
        }

        public async Task<ResponseDto> GetByIdAsync(string taskListId)
        {
            await EnsureAccessAsync(taskListId);
            var taskList = await _taskListRepo.GetByIdAsync(taskListId);
            return _mapper.Map<ResponseDto>(taskList);
        }

        public async Task<PagedResult<SummaryDto>> GetUserListsAsync(int page, int pageSize)
        {
            var userId = _currentUserService.GetCurrentUserId();
            // get lists where user is owner or has access
            int skip = (page - 1) * pageSize;
            var owned = await _taskListRepo.GetByOwnerIdAsync(userId, skip, pageSize);
            var shared = await _taskListRepo.GetBySharedWithUserIdAsync(userId, skip, pageSize);

            // We combine, sort by creation time (from newest to oldest)
            var all = owned.Concat(shared)
                .GroupBy(x => x.Id)
                .Select(g => g.First())
                .OrderByDescending(x => x.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .ToList();

            var totalCount = all.Count(); 

            var items = _mapper.Map<IEnumerable<SummaryDto>>(all);
            return new PagedResult<SummaryDto>(items, totalCount, page, pageSize);
        }

        public async Task AddShareAsync(string taskListId, string targetUserId)
        {
            var taskList = await EnsureAccessAsync(taskListId);

            if (taskList.OwnerId == targetUserId)
                throw new Exception("Cannot share with the owner.");

            if (taskList.SharedWithUserIds.Contains(targetUserId))
                throw new Exception("User already has access.");

            await _taskListRepo.AddShareAsync(taskListId, targetUserId);
        }

        public async Task<IEnumerable<string>> GetSharesAsync(string taskListId)
        {
            await EnsureAccessAsync(taskListId);
            return await _taskListRepo.GetSharedUserIdsAsync(taskListId);
        }

        public async Task RemoveShareAsync(string taskListId, string targetUserId)
        {
            await EnsureAccessAsync(taskListId);
            await _taskListRepo.RemoveShareAsync(taskListId, targetUserId);
        }
    }
}
