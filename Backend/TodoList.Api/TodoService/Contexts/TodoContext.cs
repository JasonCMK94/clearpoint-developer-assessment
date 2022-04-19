using Microsoft.EntityFrameworkCore;

namespace TodoList.Api
{
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore.Internal;

    public interface ITodoContext
    {
        DbSet<TodoItem> TodoItems { get; set; }

        Task<TodoItem> Update(TodoItem todoItem);

        Task<TodoItem> Insert(TodoItem todoItem);
    }

    public class TodoContext : DbContext, ITodoContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }

        public DbSet<TodoItem> TodoItems { get; set; }

        public async Task<TodoItem> Update(TodoItem todoItem)
        {
            Attach(todoItem);
            Entry(todoItem).State = EntityState.Modified;

            await SaveChangesAsync();

            return todoItem;
        }

        public async Task<TodoItem> Insert(TodoItem todoItem)
        {
            TodoItems.Add(todoItem);
            await SaveChangesAsync();

            return todoItem;
        }
    }
}
