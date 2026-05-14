// Data Transfer Objects (DTOs) for API communication
namespace todoListAngular.Server.Todos;

// Response DTO sent to the client (matches TodoItem structure)
public sealed record TodoItemDto(Guid Id, string Title, DateTimeOffset CreatedAt);

// Request DTO for creating a new TODO item
public sealed record CreateTodoRequest(string Title);