namespace TodoList.Api.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    using Moq;

    using TodoList.Api.Controllers;

    using Xunit;

    public class TodoItemsControllerTests
    {
        private static DbContextOptions<TodoContext> ContextOptions =>
            new DbContextOptionsBuilder<TodoContext>().UseInMemoryDatabase("TodoItemsDB").Options;

        private static TodoContext GetContext()
        {
            var context = new TodoContext(ContextOptions);
            context.RemoveRange(context.TodoItems);
            return context;
        }

        [Fact]
        public async void TodoItemGet_ShouldReturnItems()
        {
            // Arrange
            var context = GetContext();
            var descriptions = new List<string>{ "first", "second" };
            context.AddRange(descriptions.Select(x => new TodoItem { Description = x }));
            context.SaveChanges();
            var controller = new TodoItemsController(context, new Mock<ILogger<TodoItemsController>>().Object);

            // Act
            var items = (List<TodoItem>)((OkObjectResult)await controller.GetTodoItems()).Value;

            // Assert
            Assert.Equal(2, items.Count);
            Assert.All(items, x => Assert.Contains(x.Description, descriptions));
        }

        [Fact]
        public async void TodoItemGetSingle_ShouldReturnItem()
        {
            // Arrange
            var context = GetContext();
            Guid id = new Guid("9543DC12-DB45-4066-AB09-81E04F70C929");
            context.Add(new TodoItem
                            {
                                Id = id,
                                Description = "single item"
                            });
            context.SaveChanges();
            var controller = new TodoItemsController(context, new Mock<ILogger<TodoItemsController>>().Object);

            // Act
            var item = (TodoItem)((OkObjectResult)await controller.GetTodoItem(id)).Value;

            // Assert
            Assert.Equal(id, item.Id);
            Assert.Equal("single item", item.Description);
        }

        [Fact]
        public async void TodoItemGetSingleMissing_ShouldReturnNotFound()
        {
            // Arrange
            var context = GetContext();
            Guid id = new Guid("9543DC12-DB45-4066-AB09-81E04F70C929");
            var controller = new TodoItemsController(context, new Mock<ILogger<TodoItemsController>>().Object);

            // Act
            var result = (NotFoundResult)await controller.GetTodoItem(id);

            // Assert
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async void TodoItemPut_ShouldUpdateItem()
        {
            // Arrange
            var context = GetContext();
            Guid id = new Guid("9543DC12-DB45-4066-AB09-81E04F70C929");
            context.Add(new TodoItem { Id = id, Description = "single item" });
            context.SaveChanges();

            var controller = new TodoItemsController(
                new TodoContext(ContextOptions),
                new Mock<ILogger<TodoItemsController>>().Object);

            // Act
            await controller.PutTodoItem(
                id,
                new TodoItem { Description = "updated description", Id = id, IsCompleted = true });

            // Assert
            var updatedItem = new TodoContext(ContextOptions).TodoItems.First();
            Assert.Equal(id, updatedItem.Id);
            Assert.Equal("updated description", updatedItem.Description);
            Assert.True(updatedItem.IsCompleted);
        }

        [Fact]
        public async void TodoItemPutDeleted_ShouldReturnNotFound()
        {
            // Arrange
            Guid id = new Guid("AAAE5CA8-9D63-44A3-B30A-0AD06755B227");

            var controller = new TodoItemsController(
                new TodoContext(ContextOptions),
                new Mock<ILogger<TodoItemsController>>().Object);

            // Act
            var result = (NotFoundResult)await controller.PutTodoItem(
                                             id,
                                             new TodoItem
                                                 {
                                                     Description = "updated description", Id = id, IsCompleted = true
                                                 });

            // Assert
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async void TodoItemPutMismatchedId_ReturnBadRequest()
        {
            // Arrange
            var context = GetContext();
            Guid id = new Guid("9543DC12-DB45-4066-AB09-81E04F70C929");
            context.Add(new TodoItem { Id = id, Description = "single item" });
            context.SaveChanges();

            var controller = new TodoItemsController(context, new Mock<ILogger<TodoItemsController>>().Object);

            // Act
            var result = (BadRequestResult)await controller.PutTodoItem(
                                           new Guid("C5DF58C8-27A2-4B67-8365-EE6DC239ED8F"),
                                           new TodoItem
                                               {
                                                   Description = "updated description", Id = id, IsCompleted = true
                                               });

            // Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async void TodoItemPost_ShouldCreateItem()
        {
            // Arrange
            var context = GetContext();
            var controller = new TodoItemsController(context, new Mock<ILogger<TodoItemsController>>().Object);

            // Act
            await controller.PostTodoItem(new TodoItem { Description = "test description" });

            // Assert
            var todoItem = context.TodoItems.First();
            Assert.Equal("test description", todoItem.Description);
        }

        [Fact]
        public async void DuplicateTodoItemPost_ShouldReturnBadRequest()
        {
            // Arrange
            var context = GetContext();
            var controller = new TodoItemsController(context, new Mock<ILogger<TodoItemsController>>().Object);
            await controller.PostTodoItem(new TodoItem { Description = "test description" });

            // Act
            var result = (ObjectResult)await controller.PostTodoItem(new TodoItem { Description = "test description" });

            // Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }
    }
}
