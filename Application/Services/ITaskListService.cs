using Application.DTOs;

namespace Application.Services
{
    public interface ITaskListService
    {
        Task<ResponseDto> CreateAsync(CreateDto dto);
        Task<ResponseDto> UpdateAsync(UpdateDto dto);
        Task DeleteAsync(string taskListId);
        Task<ResponseDto> GetByIdAsync(string taskListId);
        Task<PagedResult<SummaryDto>> GetUserListsAsync(int page, int pageSize);
        Task AddShareAsync(string taskListId, string targetUserId);
        Task<IEnumerable<string>> GetSharesAsync(string taskListId);
        Task RemoveShareAsync(string taskListId, string targetUserId);
    }
}
