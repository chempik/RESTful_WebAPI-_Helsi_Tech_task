using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.DTOs;

namespace RESTful_WebAPI__Helsi_Tech_task.Controllers
{
    [ApiController]
    [Route("api/tasklists")]
    //public class TaskListsController : Controller
    public class TaskListsController : ControllerBase
    {
        private readonly ITaskListService _taskListService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TaskListsController(ITaskListService taskListService, IHttpContextAccessor httpContextAccessor)
        {
            _taskListService = taskListService;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetCurrentUserId()
        {
            // Getting userId from the header
            var userId = _httpContextAccessor.HttpContext?.Request.Headers["X-UserId"].FirstOrDefault();
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User ID is required.");
            return userId;
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] CreateDto dto)
        {
            var result = await _taskListService.CreateAsync(GetCurrentUserId(), dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseDto>> Update(string id, [FromBody] UpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch.");
            var result = await _taskListService.UpdateAsync(GetCurrentUserId(), dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _taskListService.DeleteAsync(GetCurrentUserId(), id);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto>> GetById(string id)
        {
            var result = await _taskListService.GetByIdAsync(GetCurrentUserId(), id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<SummaryDto>>> GetUserLists([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _taskListService.GetUserListsAsync(GetCurrentUserId(), page, pageSize);
            return Ok(result);
        }

        [HttpPost("{id}/share")]
        public async Task<IActionResult> AddShare(string id, [FromBody] ShareDto dto)
        {
            await _taskListService.AddShareAsync(GetCurrentUserId(), id, dto.UserId);
            return NoContent();
        }

        [HttpGet("{id}/share")]
        public async Task<ActionResult<IEnumerable<string>>> GetShares(string id)
        {
            var result = await _taskListService.GetSharesAsync(GetCurrentUserId(), id);
            return Ok(result);
        }

        [HttpDelete("{id}/share/{userId}")]
        public async Task<IActionResult> RemoveShare(string id, string userId)
        {
            await _taskListService.RemoveShareAsync(GetCurrentUserId(), id, userId);
            return NoContent();
        }
    }
}
