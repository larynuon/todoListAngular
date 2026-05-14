// API Controller handling HTTP requests for TODO operations
using Microsoft.AspNetCore.Mvc;
using todoListAngular.Server.Todos;

namespace todoListAngular.Server.Controllers
{
    [ApiController]
    [Route("api/todos")]  // Base route: /api/todos
    public sealed class TodosController : ControllerBase
    {
        private readonly TodoService _service;

        public TodosController(TodoService service) => _service = service;

        // GET /api/todos - Retrieve all TODO items
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<TodoItemDto>>> GetAll(CancellationToken ct)
        {
            var items = await _service.GetAllAsync(ct);
            return Ok(items.Select(ToDto).ToList());  // Convert domain models to DTOs
        }

        // POST /api/todos - Create a new TODO item
        [HttpPost]
        public async Task<ActionResult<TodoItemDto>> Create([FromBody] CreateTodoRequest request, CancellationToken ct)
        {
            try
            {
                var item = await _service.AddAsync(request.Title, ct);
                // Return 201 Created with location header
                return CreatedAtAction(nameof(GetAll), new { }, ToDto(item));
            }
            catch (ArgumentException ex)
            {
                // Return 400 Bad Request for validation errors
                return ValidationProblem(new ValidationProblemDetails
                {
                    Title = "Validation error",
                    Detail = ex.Message
                });
            }
        }

        // DELETE /api/todos/{id} - Delete a TODO item by ID
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var removed = await _service.DeleteAsync(id, ct);
            return removed ? NoContent() : NotFound();  // 204 if deleted, 404 if not found
        }

        // Helper method to convert domain model to DTO
        private static TodoItemDto ToDto(TodoItem x) => new(x.Id, x.Title, x.CreatedAt);
    }
}
