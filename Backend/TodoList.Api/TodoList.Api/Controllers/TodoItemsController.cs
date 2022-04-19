using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TodoList.Api.Controllers
{
    using System.Text.Json;

    using TodoList.Api.Helpers;

    [Route("api/v1/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoContext _context;
        private readonly ILogger<TodoItemsController> _logger;

        public TodoItemsController(ITodoContext context, ILogger<TodoItemsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<IActionResult> GetTodoItems()
        {
            var results = await _context.TodoItems.Where(x => !x.IsCompleted).ToListAsync();
            _logger.Log(
                LogLevel.Information,
                "Items loaded\n" + string.Join("\n", JsonSerializer.Serialize(results)));
            return Ok(results);
        }

        // GET: api/TodoItems/...
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoItem(Guid id)
        {
            var result = await _context.TodoItems.FindAsync(id);
            if (result == null)
            {
                _logger.Log(LogLevel.Error, $"Failed to retrieve item {id}");
                return NotFound();
            }

            _logger.Log(LogLevel.Information, $"Item {JsonSerializer.Serialize(result)} retrieved");
            return Ok(result);
        }

        // PUT: api/TodoItems/... 
        [HttpPut]
        public async Task<IActionResult> PutTodoItem(TodoItem todoItem)
        {
            if (!_context.TodoItems.Any(x => x.Id == todoItem.Id))
            {
                _logger.Log(LogLevel.Error, $"Failed to update item {JsonSerializer.Serialize(todoItem)}");
                return NotFound();
            }

            todoItem = await _context.Update(todoItem);
            _logger.Log(LogLevel.Information, $"Item {JsonSerializer.Serialize(todoItem)} updated");

            return Ok(todoItem);
        } 

        // POST: api/TodoItems 
        [HttpPost]
        public async Task<IActionResult> PostTodoItem(TodoItem todoItem)
        {
            if (!TodoItemValidator.IsValidToCreate(todoItem, _context, out BadRequestObjectResult badRequest))
            {
                _logger.Log(LogLevel.Error, badRequest.Value + "\n" + JsonSerializer.Serialize(todoItem));
                return badRequest;
            }

            await _context.Insert(todoItem);
            _logger.Log(LogLevel.Information, $"Item {JsonSerializer.Serialize(todoItem)} added.");

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }
    }
}
