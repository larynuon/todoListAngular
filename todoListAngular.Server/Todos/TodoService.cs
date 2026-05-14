// Business logic service for TODO operations (validation and orchestration)
namespace todoListAngular.Server.Todos
{
    public sealed class TodoService
    {
        private readonly ITodoRepository _repo;

        public TodoService(ITodoRepository repo) => _repo = repo;

        // Get all TODO items from the repository
        public Task<IReadOnlyList<TodoItem>> GetAllAsync(CancellationToken ct)
            => _repo.GetAllAsync(ct);

        // Add a new TODO item with validation
        public async Task<TodoItem> AddAsync(string title, CancellationToken ct)
        {
            // Validate that title is not empty
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required.", nameof(title));

            // Validate title length (max 200 characters)
            if (title.Trim().Length > 200)
                throw new ArgumentException("Title must be 200 characters or less.", nameof(title));

            // Delegate to repository after validation passes
            return await _repo.AddAsync(title, ct);
        }

        // Delete a TODO item by ID
        public Task<bool> DeleteAsync(Guid id, CancellationToken ct)
            => _repo.DeleteAsync(id, ct);
    }
}
