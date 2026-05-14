// Repository interface defining data access operations for TODO items
namespace todoListAngular.Server.Todos
{
    public interface ITodoRepository
    {
        // Retrieve all TODO items (ordered by creation date descending)
        Task<IReadOnlyList<TodoItem>> GetAllAsync(CancellationToken ct);

        // Create and store a new TODO item
        Task<TodoItem> AddAsync(string title, CancellationToken ct);

        // Delete a TODO item by ID (returns true if found and deleted)
        Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    }
}
