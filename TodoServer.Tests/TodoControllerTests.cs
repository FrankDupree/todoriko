using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TodoServer.Application.Interfaces;
using TodoServer.Controllers;
using TodoServer.Entities;
using TodoServer.Entities.Dtos;
using TodoServer.Entities.Enums;
using ValidationException = FluentValidation.ValidationException;

namespace TodoServer.Tests
{
    public class TodoControllerTests
    {
        private readonly Mock<ITodoService> _mockService;
        private readonly TodosController _controller;

        public TodoControllerTests()
        {
            _mockService = new Mock<ITodoService>();
            _controller = new TodosController(_mockService.Object,Mock.Of<IMapper>(),Mock.Of<ILogger<TodosController>>());

        }

        [Fact]
        public async Task GetTodo_ExistingId_ReturnsTodo()
        {
            // Arrange
            var testTodo = new Todo { Id = Guid.NewGuid(), Title = "Test" };
            _mockService.Setup(x => x.GetTodoByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(testTodo);

            // Act
            var result = await _controller.GetById(testTodo.Id);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(testTodo);
        }


        [Fact]
        public async Task UpdateTodo_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var testId = Guid.NewGuid();
            var updateDto = new UpdateTodoDto
            {
                Title = "Test",
                Priority = PriorityLevel.Medium
            };

            var mockValidator = new Mock<IValidator<UpdateTodoDto>>();
            mockValidator
                .Setup(x => x.ValidateAsync(updateDto, default))
                .ReturnsAsync(new ValidationResult()); // Simulate passing validation

            _mockService
                .Setup(x => x.UpdateTodoAsync(testId, updateDto))
                .ThrowsAsync(new ValidationException(new List<FluentValidation.Results.ValidationFailure>
                {
            new("DueDate", "Due date must be today or in the future")
                }));

            var controller = new TodosController(_mockService.Object, Mock.Of<IMapper>(), Mock.Of<ILogger<TodosController>>());

            // Act
            var result = await controller.Update(testId, updateDto, mockValidator.Object);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

    }
}
