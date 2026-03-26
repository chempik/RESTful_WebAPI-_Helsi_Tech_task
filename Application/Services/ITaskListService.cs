using Application.DTOs;
using Domain.Interfaces;
using Domain.Entities;
using System.Linq.Dynamic.Core;

namespace Application.Services
{
    public interface ITaskListService
    {
        Task<ResponseDto> CreateAsync(string currentUserId, CreateDto dto);
        Task<ResponseDto> UpdateAsync(string currentUserId, UpdateDto dto);
        Task DeleteAsync(string currentUserId, string taskListId);
        Task<ResponseDto> GetByIdAsync(string currentUserId, string taskListId);
        Task<PagedResult<SummaryDto>> GetUserListsAsync(string currentUserId, int page, int pageSize);
        Task AddShareAsync(string currentUserId, string taskListId, string targetUserId);
        Task<IEnumerable<string>> GetSharesAsync(string currentUserId, string taskListId);
        Task RemoveShareAsync(string currentUserId, string taskListId, string targetUserId);
    }
}
