// Unit tests for InMemoryTodoRepository
using FluentAssertions;
using todoListAngular.Server.Todos;

namespace todoListAngular.Server.Tests
{
    public class InMemoryTodoRepositoryTests
    {
        private readonly InMemoryTodoRepository _repository;

        public InMemoryTodoRepositoryTests()
        {
            _repository = new InMemoryTodoRepository();
        }

        [Fact]
        public async Task GetAllAsync_WhenEmpty_ShouldReturnEmptyList()
        {
            // Act
            var result = await _repository.GetAllAsync(CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task AddAsync_ShouldCreateNewItem()
        {
            // Arrange
            var title = "Test TODO";

            // Act
            var result = await _repository.AddAsync(title, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeEmpty();
            result.Title.Should().Be(title);
            result.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task AddAsync_ShouldTrimTitle()
        {
            // Arrange
            var title = "  Test TODO  ";

            // Act
            var result = await _repository.AddAsync(title, CancellationToken.None);

            // Assert
            result.Title.Should().Be("Test TODO");
        }

        [Fact]
        public async Task AddAsync_MultipleItems_ShouldStoreAll()
        {
            // Arrange & Act
            var item1 = await _repository.AddAsync("Item 1", CancellationToken.None);
            var item2 = await _repository.AddAsync("Item 2", CancellationToken.None);

            // Assert
            var allItems = await _repository.GetAllAsync(CancellationToken.None);
            allItems.Should().HaveCount(2);
            allItems.Should().Contain(i => i.Id == item1.Id);
            allItems.Should().Contain(i => i.Id == item2.Id);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnItemsOrderedByNewestFirst()
        {
            // Arrange
            var item1 = await _repository.AddAsync("Item 1", CancellationToken.None);
            await Task.Delay(10); // Small delay to ensure different timestamps
            var item2 = await _repository.AddAsync("Item 2", CancellationToken.None);
            await Task.Delay(10);
            var item3 = await _repository.AddAsync("Item 3", CancellationToken.None);

            // Act
            var result = await _repository.GetAllAsync(CancellationToken.None);

            // Assert
            result.Should().HaveCount(3);
            result[0].Id.Should().Be(item3.Id); // Newest first
            result[1].Id.Should().Be(item2.Id);
            result[2].Id.Should().Be(item1.Id); // Oldest last
        }

        [Fact]
        public async Task DeleteAsync_ExistingItem_ShouldReturnTrueAndRemoveItem()
        {
            // Arrange
            var item = await _repository.AddAsync("Test TODO", CancellationToken.None);

            // Act
            var result = await _repository.DeleteAsync(item.Id, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            var allItems = await _repository.GetAllAsync(CancellationToken.None);
            allItems.Should().BeEmpty();
        }

        [Fact]
        public async Task DeleteAsync_NonExistingItem_ShouldReturnFalse()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.DeleteAsync(nonExistentId, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteAsync_ShouldOnlyRemoveSpecifiedItem()
        {
            // Arrange
            var item1 = await _repository.AddAsync("Item 1", CancellationToken.None);
            var item2 = await _repository.AddAsync("Item 2", CancellationToken.None);
            var item3 = await _repository.AddAsync("Item 3", CancellationToken.None);

            // Act
            await _repository.DeleteAsync(item2.Id, CancellationToken.None);

            // Assert
            var allItems = await _repository.GetAllAsync(CancellationToken.None);
            allItems.Should().HaveCount(2);
            allItems.Should().Contain(i => i.Id == item1.Id);
            allItems.Should().Contain(i => i.Id == item3.Id);
            allItems.Should().NotContain(i => i.Id == item2.Id);
        }

        [Fact]
        public async Task Repository_ShouldBeThreadSafe()
        {
            // Arrange
            var tasks = new List<Task>();

            // Act - Add items concurrently
            for (int i = 0; i < 100; i++)
            {
                var index = i;
                tasks.Add(Task.Run(async () => await _repository.AddAsync($"Item {index}", CancellationToken.None)));
            }
            await Task.WhenAll(tasks);

            // Assert
            var allItems = await _repository.GetAllAsync(CancellationToken.None);
            allItems.Should().HaveCount(100);
            allItems.Select(i => i.Id).Distinct().Should().HaveCount(100); // All unique IDs
        }
    }
}
