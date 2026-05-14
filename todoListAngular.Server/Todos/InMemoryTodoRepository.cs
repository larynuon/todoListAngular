// In-memory implementation of the TODO repository (for development/testing)
using System.Collections.Concurrent;

namespace todoListAngular.Server.Todos
{
    public sealed class InMemoryTodoRepository : ITodoRepository
    {
        // Thread-safe dictionary to store TODO items in memory
        private readonly ConcurrentDictionary<Guid, TodoItem> _store = new();

        // Get all TODO items sorted by newest first
        public Task<IReadOnlyList<TodoItem>> GetAllAsync(CancellationToken ct)
            => Task.FromResult<IReadOnlyList<TodoItem>>(_store.Values
                .OrderByDescending(x => x.CreatedAt)
                .ToList());

        // Add a new TODO item with a generated ID and current timestamp
        public Task<TodoItem> AddAsync(string title, CancellationToken ct)
        {
            var item = new TodoItem(Guid.NewGuid(), title.Trim(), DateTimeOffset.UtcNow);
            _store[item.Id] = item;  // Store in dictionary
            return Task.FromResult(item);
        }

        // Remove a TODO item by ID (returns true if item existed)
        public Task<bool> DeleteAsync(Guid id, CancellationToken ct)
            => Task.FromResult(_store.TryRemove(id, out _));
    }
}
