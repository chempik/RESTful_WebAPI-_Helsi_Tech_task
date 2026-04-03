using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.DTOs;

namespace RESTful_WebAPI__Helsi_Tech_task.Controllers
{
    [ApiController]
    [Route("api/tasklists")]
    public class TaskListsController : ControllerBase
    {
        private readonly ITaskListService _taskListService;

        public TaskListsController(ITaskListService taskListService)
        {
            _taskListService = taskListService;
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDto>> Create([FromBody] CreateDto dto)
        {
            var result = await _taskListService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseDto>> Update(string id, [FromBody] UpdateDto dto)
        {
            var result = await _taskListService.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _taskListService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto>> GetById(string id)
        {
            var result = await _taskListService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<SummaryDto>>> GetUserLists([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _taskListService.GetUserListsAsync(page, pageSize);
            return Ok(result);
        }

        [HttpPost("{id}/share")]
        public async Task<IActionResult> AddShare(string id, [FromBody] ShareDto dto)
        {
            await _taskListService.AddShareAsync(id, dto.UserId);
            return NoContent();
        }

        [HttpGet("{id}/share")]
        public async Task<ActionResult<IEnumerable<string>>> GetShares(string id)
        {
            var result = await _taskListService.GetSharesAsync(id);
            return Ok(result);
        }

        [HttpDelete("{id}/share/{userId}")]
        public async Task<IActionResult> RemoveShare(string id, string userId)
        {
            await _taskListService.RemoveShareAsync(id, userId);
            return NoContent();
        }
    }
}
