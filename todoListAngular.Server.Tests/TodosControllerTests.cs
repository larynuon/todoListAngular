// Unit tests for TodosController API endpoints
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using todoListAngular.Server.Controllers;
using todoListAngular.Server.Todos;

namespace todoListAngular.Server.Tests
{
    public class TodosControllerTests
    {
        private readonly Mock<ITodoRepository> _repositoryMock;
        private readonly TodoService _service;
        private readonly TodosController _controller;

        public TodosControllerTests()
        {
            _repositoryMock = new Mock<ITodoRepository>();
            _service = new TodoService(_repositoryMock.Object);
            _controller = new TodosController(_service);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithItems()
        {
            // Arrange
            var items = new List<TodoItem>
            {
                new(Guid.NewGuid(), "Item 1", DateTimeOffset.UtcNow),
                new(Guid.NewGuid(), "Item 2", DateTimeOffset.UtcNow)
            };
            _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(items);

            // Act
            var result = await _controller.GetAll(CancellationToken.None);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedItems = okResult.Value.Should().BeAssignableTo<IEnumerable<TodoItemDto>>().Subject;
            returnedItems.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAll_WhenEmpty_ShouldReturnOkWithEmptyList()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TodoItem>());

            // Act
            var result = await _controller.GetAll(CancellationToken.None);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedItems = okResult.Value.Should().BeAssignableTo<IEnumerable<TodoItemDto>>().Subject;
            returnedItems.Should().BeEmpty();
        }

        [Fact]
        public async Task Create_WithValidRequest_ShouldReturnCreated()
        {
            // Arrange
            var request = new CreateTodoRequest("New TODO");
            var createdItem = new TodoItem(Guid.NewGuid(), "New TODO", DateTimeOffset.UtcNow);
            _repositoryMock.Setup(r => r.AddAsync(request.Title, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdItem);

            // Act
            var result = await _controller.Create(request, CancellationToken.None);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(TodosController.GetAll));
            var returnedDto = createdResult.Value.Should().BeOfType<TodoItemDto>().Subject;
            returnedDto.Title.Should().Be("New TODO");
            returnedDto.Id.Should().Be(createdItem.Id);
        }

        [Fact]
        public async Task Create_WithInvalidTitle_ShouldReturnValidationProblem()
        {
            // Arrange
            var request = new CreateTodoRequest("");

            // Act
            var result = await _controller.Create(request, CancellationToken.None);

            // Assert
            var problemResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            problemResult.StatusCode.Should().Be(400);
            var problemDetails = problemResult.Value.Should().BeOfType<ValidationProblemDetails>().Subject;
            problemDetails.Title.Should().Be("Validation error");
            problemDetails.Detail.Should().Contain("Title is required");
        }

        [Fact]
        public async Task Create_WithTooLongTitle_ShouldReturnValidationProblem()
        {
            // Arrange
            var longTitle = new string('a', 201);
            var request = new CreateTodoRequest(longTitle);

            // Act
            var result = await _controller.Create(request, CancellationToken.None);

            // Assert
            var problemResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            problemResult.StatusCode.Should().Be(400);
            var problemDetails = problemResult.Value.Should().BeOfType<ValidationProblemDetails>().Subject;
            problemDetails.Detail.Should().Contain("200 characters or less");
        }

        [Fact]
        public async Task Delete_ExistingItem_ShouldReturnNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repositoryMock.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(id, CancellationToken.None);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_NonExistingItem_ShouldReturnNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repositoryMock.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(id, CancellationToken.None);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetAll_ShouldCallRepositoryOnce()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TodoItem>());

            // Act
            await _controller.GetAll(CancellationToken.None);

            // Assert
            _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldCallRepositoryOnce()
        {
            // Arrange
            var request = new CreateTodoRequest("Test");
            var item = new TodoItem(Guid.NewGuid(), "Test", DateTimeOffset.UtcNow);
            _repositoryMock.Setup(r => r.AddAsync(request.Title, It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);

            // Act
            await _controller.Create(request, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(r => r.AddAsync(request.Title, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldCallRepositoryOnce()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repositoryMock.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _controller.Delete(id, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
