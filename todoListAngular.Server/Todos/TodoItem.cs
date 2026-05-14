// Domain model representing a TODO item in the system
namespace todoListAngular.Server.Todos;

// Immutable record with ID, title, and creation timestamp
public sealed record TodoItem(Guid Id, string Title, DateTimeOffset CreatedAt);