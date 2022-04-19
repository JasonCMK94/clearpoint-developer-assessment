namespace TodoList.Api.Helpers
{
    using System.Linq;

    using Microsoft.AspNetCore.Mvc;

    public static class TodoItemValidator
    {
        public static bool IsValidToCreate(TodoItem todoItem, ITodoContext context, out BadRequestObjectResult badRequest)
        {
            if (string.IsNullOrEmpty(todoItem?.Description))
            {
                badRequest = new BadRequestObjectResult("Description is required");
                return false;
            }

            if (context.TodoItems.Any(
                x => x.Description.ToLowerInvariant() == todoItem.Description.ToLowerInvariant() && !x.IsCompleted))
            {
                badRequest = new BadRequestObjectResult("Description already exists");
                return false;
            }

            badRequest = null;
            return true;
        }
    }
}
