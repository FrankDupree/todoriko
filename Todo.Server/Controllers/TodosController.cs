using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using TodoServer.Application.Interfaces;
using TodoServer.Entities;
using TodoServer.Entities.Dtos;

namespace TodoServer.Controllers
{
    /// <summary>
    /// API endpoints for managing Todo items
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]

    public class TodosController : ControllerBase
    {
        private readonly ITodoService _todoService;
        private readonly ILogger<TodosController> _logger;
        private readonly IMapper _mapper;


        public TodosController(
            ITodoService todoService,
             IMapper mapper,
            ILogger<TodosController> logger)
        {
            _todoService = todoService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get paginated and filtered list of todo items
        /// </summary>
        /// <param name="request">Pagination and filtering parameters</param>
        /// <returns>Paginated list of todo items with metadata</returns>
        /// <response code="200">Returns the paginated list of todos</response>
        /// <response code="400">If the request parameters are invalid</response>
        [HttpGet]
        [ProducesResponseType(typeof(TodoListResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTodos([FromQuery] TodoListRequestDto request)
        {
            _logger.LogInformation("Getting todos with filters: {@Request}", request);

            try
            {
                var result = await _todoService.GetTodosAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting todos");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get a specific todo item by ID
        /// </summary>
        /// <param name="id">The ID of the todo item</param>
        /// <returns>The requested todo item</returns>
        /// <response code="200">Returns the requested todo</response>
        /// <response code="404">If the todo item is not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Todo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            _logger.LogInformation("Getting todo with ID: {Id}", id);
            try
            {
                var todo = await _todoService.GetTodoByIdAsync(id);
                return Ok(todo);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Todo with ID {Id} not found", id);
                return NotFound();
            }
        }

        /// <summary>
        /// Create a new todo item
        /// </summary>
        /// <param name="todo">The todo item to create</param>
        /// <returns>The created todo item</returns>
        /// <response code="201">Returns the newly created todo</response>
        /// <response code="400">If the todo item is invalid</response>
        [HttpPost]
        [ProducesResponseType(typeof(TodoItemDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateTodoDto createDto)
        {
            _logger.LogInformation("Creating new todo");
            try
            {
                var todo = _mapper.Map<Todo>(createDto);
                var createdTodo = await _todoService.CreateTodoAsync(todo);
                return CreatedAtAction(nameof(GetById), new { id = createdTodo.Id }, createdTodo);
            }
            catch (ValidationException ex)
            {
                var errors = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.ErrorMessage).ToArray()
                    );

                return BadRequest(new
                {
                    errors,
                    type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    title = "Validation Error",
                    status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Update an existing todo item
        /// </summary>
        /// <param name="id">The ID of the todo item to update</param>
        /// <param name="todo">The updated todo item data</param>
        /// <returns>No content</returns>
        /// <response code="204">If the update was successful</response>
        /// <response code="400">If the todo item is invalid</response>
        /// <response code="404">If the todo item is not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTodoDto updateDto, [FromServices] IValidator<UpdateTodoDto> validator)
        {
            _logger.LogInformation("Updating todo with ID: {Id}", id);

            var validationResult = await validator.ValidateAsync(updateDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.ToDictionary();
                return ValidationProblem(new ValidationProblemDetails(errors));
            }

            try
            {
                await _todoService.UpdateTodoAsync(id, updateDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Todo with ID {Id} not found", id);
                return NotFound();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed when updating todo");
                var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );
                return BadRequest(new
                {
                    errors,
                    type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    title = "Validation Error",
                    status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Mark a todo item as completed
        /// </summary>
        /// <param name="id">The ID of the todo item to complete</param>
        /// <returns>No content</returns>
        /// <response code="204">If the operation was successful</response>
        /// <response code="404">If the todo item is not found</response>
        [HttpPatch("{id}/complete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkAsCompleted(Guid id)
        {
            _logger.LogInformation("Marking todo with ID {Id} as completed", id);
            try
            {
                var todo = await _todoService.GetTodoByIdAsync(id);
                todo.IsCompleted = !todo.IsCompleted;
                await _todoService.UpdateTodoAsync(todo);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Todo with ID {Id} not found", id);
                return NotFound();
            }
        }

        /// <summary>
        /// Delete a todo item (soft delete)
        /// </summary>
        /// <param name="id">The ID of the todo item to delete</param>
        /// <returns>No content</returns>
        /// <response code="204">If the deletion was successful</response>
        /// <response code="404">If the todo item is not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Soft deleting todo with ID: {Id}", id);
            try
            {
                await _todoService.SoftDeleteTodoAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Todo with ID {Id} not found", id);
                return NotFound();
            }
        }

        /// <summary>
        /// Permanently delete a todo item
        /// </summary>
        /// <param name="id">The ID of the todo item to permanently delete</param>
        /// <returns>No content</returns>
        /// <response code="204">If the permanent deletion was successful</response>
        /// <response code="404">If the todo item is not found</response>
        [HttpDelete("{id}/permanent")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PermanentDelete(Guid id)
        {
            _logger.LogInformation("Permanently deleting todo with ID: {Id}", id);
            try
            {
                await _todoService.HardDeleteTodoAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Todo with ID {Id} not found", id);
                return NotFound();
            }
        }
    }
}
