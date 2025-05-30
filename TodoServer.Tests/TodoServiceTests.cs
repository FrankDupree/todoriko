using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoServer.Application.Services;
using TodoServer.Entities.Interfaces;
using TodoServer.Entities;
using FluentValidation;
using AutoMapper;
using FluentAssertions;
using FluentValidation.Results;
using TodoServer.Entities.Dtos;
using TodoServer.Entities.Enums;

namespace TodoServer.Tests
{
    public class TodoServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly TodoService _service;
        private readonly Mock<IValidator<Todo>> _mockValidator;


        public TodoServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockValidator = new Mock<IValidator<Todo>>();
            _service = new TodoService(_mockUnitOfWork.Object, Mock.Of<IValidator<Todo>>(), Mock.Of<IMapper>());
        }


        [Fact]
        public async Task CreateTodo_ValidData_ReturnsCreatedTodo()
        {
            var todoToCreate = new Todo
            {
                Id = Guid.NewGuid(), 
                Title = "My New Task",
                Tag = "Work",
                Description = "A detailed description of the task.",
                IsCompleted = false,
                Priority = PriorityLevel.Medium,
                CreatedAt = DateTime.UtcNow 
            };

            var mockValidator = new Mock<IValidator<Todo>>();
            mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Todo>(), CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            var mockTodoRepository = new Mock<IRepository<Todo>>();
            mockTodoRepository
                .Setup(r => r.AddAsync(It.IsAny<Todo>()))
                .Returns(Task.CompletedTask);

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork
                .Setup(uow => uow.TodoRepository)
                .Returns(mockTodoRepository.Object);
            mockUnitOfWork
                .Setup(uow => uow.CompleteAsync())
                .ReturnsAsync(1);

            var mockMapper = new Mock<IMapper>();
            mockMapper
                .Setup(m => m.Map<TodoItemDto>(It.IsAny<Todo>()))
                .Returns((Todo sourceTodo) => new TodoItemDto
                {
                    Id = sourceTodo.Id,
                    Title = sourceTodo.Title,
                    Tag = sourceTodo.Tag,
                    Description = sourceTodo.Description,
                    IsCompleted = sourceTodo.IsCompleted,
                    CreatedAt = sourceTodo.CreatedAt,
                    Priority = sourceTodo.Priority,
                    DueDate = sourceTodo.DueDate
                });

            var service = new TodoService(
                mockUnitOfWork.Object,
                mockValidator.Object,
                mockMapper.Object);

            var result = await service.CreateTodoAsync(todoToCreate);

            result.Should().NotBeNull();
            result.Should().BeOfType<TodoItemDto>();

            result.Id.Should().Be(todoToCreate.Id);
            result.Title.Should().Be(todoToCreate.Title);
            result.Tag.Should().Be(todoToCreate.Tag);
            result.Description.Should().Be(todoToCreate.Description);
            result.IsCompleted.Should().Be(todoToCreate.IsCompleted);
            result.CreatedAt.Should().Be(todoToCreate.CreatedAt);
            result.Priority.Should().Be(todoToCreate.Priority);

            mockValidator.Verify(v => v.ValidateAsync(todoToCreate, CancellationToken.None), Times.Once);
            mockTodoRepository.Verify(r => r.AddAsync(todoToCreate), Times.Once);
            mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
            mockMapper.Verify(m => m.Map<TodoItemDto>(todoToCreate), Times.Once);
        }


        [Fact]
        public async Task UpdateTodo_NonExistingId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();
            var todoToUpdate = new Todo { Id = nonExistingId, Title = "Updated Title" };

            var mockValidator = new Mock<IValidator<Todo>>();
            mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Todo>(), default))
                .ReturnsAsync(new ValidationResult());

            var mockTodoRepository = new Mock<IRepository<Todo>>();
            mockTodoRepository
                .Setup(r => r.GetByIdAsync(nonExistingId))
                .ReturnsAsync((Todo)null);

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork
                .Setup(u => u.TodoRepository)
                .Returns(mockTodoRepository.Object);

            var mockMapper = new Mock<IMapper>();

            var service = new TodoService(
                mockUnitOfWork.Object,
                mockValidator.Object,
                mockMapper.Object
            );

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateTodoAsync(todoToUpdate));

            mockValidator.Verify(v => v.ValidateAsync(todoToUpdate, default), Times.Once);
            mockTodoRepository.Verify(r => r.GetByIdAsync(nonExistingId), Times.Once);
            mockTodoRepository.Verify(r => r.UpdateAsync(It.IsAny<Todo>()), Times.Never);
            mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Never);
        }


        [Fact]
        public async Task UpdateTodo_ExistingTodo_UpdatesSuccessfully()
        {
            var existingId = Guid.NewGuid();
            var existingTodo = new Todo
            {
                Id = existingId,
                Title = "Old Title",
                Description = "Old Description"
            };

            var updatedTodo = new Todo
            {
                Id = existingId,
                Title = "Updated Title",
                Description = "Updated Description"
            };

            var mockValidator = new Mock<IValidator<Todo>>();
            mockValidator
                .Setup(v => v.ValidateAsync(updatedTodo, default))
                .ReturnsAsync(new ValidationResult());

            var mockTodoRepository = new Mock<IRepository<Todo>>();
            mockTodoRepository
                .Setup(r => r.GetByIdAsync(existingId))
                .ReturnsAsync(existingTodo);

            mockTodoRepository
                .Setup(r => r.UpdateAsync(updatedTodo))
                .Returns(Task.CompletedTask);

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork
                .Setup(uow => uow.TodoRepository)
                .Returns(mockTodoRepository.Object);

            mockUnitOfWork
                .Setup(uow => uow.CompleteAsync())
                .ReturnsAsync(1);

            var service = new TodoService(mockUnitOfWork.Object, mockValidator.Object, Mock.Of<IMapper>());

            await service.UpdateTodoAsync(updatedTodo);

            mockValidator.Verify(v => v.ValidateAsync(updatedTodo, default), Times.Once);
            mockTodoRepository.Verify(r => r.GetByIdAsync(existingId), Times.Once);
            mockTodoRepository.Verify(r => r.UpdateAsync(updatedTodo), Times.Once);
            mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
        }


    }
}
