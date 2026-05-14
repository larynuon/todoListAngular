// Unit tests for TodoService business logic
using FluentAssertions;
using Moq;
using todoListAngular.Server.Todos;

namespace todoListAngular.Server.Tests
{
    public class TodoServiceTests
    {
        private readonly Mock<ITodoRepository> _repositoryMock;
        private readonly TodoService _service;

        public TodoServiceTests()
        {
            _repositoryMock = new Mock<ITodoRepository>();
            _service = new TodoService(_repositoryMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllItems()
        {
            // Arrange
            var expectedItems = new List<TodoItem>
            {
                new(Guid.NewGuid(), "Item 1", DateTimeOffset.UtcNow),
                new(Guid.NewGuid(), "Item 2", DateTimeOffset.UtcNow)
            };
            _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedItems);

            // Act
            var result = await _service.GetAllAsync(CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedItems);
            _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_WithValidTitle_ShouldCreateItem()
        {
            // Arrange
            var title = "Valid TODO";
            var expectedItem = new TodoItem(Guid.NewGuid(), title, DateTimeOffset.UtcNow);
            _repositoryMock.Setup(r => r.AddAsync(title, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedItem);

            // Act
            var result = await _service.AddAsync(title, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedItem);
            _repositoryMock.Verify(r => r.AddAsync(title, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task AddAsync_WithEmptyTitle_ShouldThrowArgumentException(string title)
        {
            // Act
            Func<Task> act = async () => await _service.AddAsync(title, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Title is required.*");
        }

        [Fact]
        public async Task AddAsync_WithTooLongTitle_ShouldThrowArgumentException()
        {
            // Arrange
            var longTitle = new string('a', 201); // 201 characters

            // Act
            Func<Task> act = async () => await _service.AddAsync(longTitle, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Title must be 200 characters or less.*");
        }

        [Fact]
        public async Task AddAsync_WithMaxLengthTitle_ShouldSucceed()
        {
            // Arrange
            var maxTitle = new string('a', 200); // Exactly 200 characters
            var expectedItem = new TodoItem(Guid.NewGuid(), maxTitle, DateTimeOffset.UtcNow);
            _repositoryMock.Setup(r => r.AddAsync(maxTitle, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedItem);

            // Act
            var result = await _service.AddAsync(maxTitle, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            _repositoryMock.Verify(r => r.AddAsync(maxTitle, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallRepository()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repositoryMock.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.DeleteAsync(id, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            _repositoryMock.Verify(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenItemNotFound_ShouldReturnFalse()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repositoryMock.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.DeleteAsync(id, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
        }
    }
}
