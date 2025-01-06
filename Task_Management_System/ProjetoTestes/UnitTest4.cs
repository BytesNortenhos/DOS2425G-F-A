using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TMS.Controller;
using TMS.Models;
using Xunit;

namespace ProjetoTestes
{
    public class TasksControllerTests
    {
        private readonly AppDbContext _context;

        public TasksControllerTests()
        {
            var services = new ServiceCollection();
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer("Server=JCOSTA_ASUS\\SQLEXPRESS;Database=DosBD;User Id=sa;Password=PasswordTeste123!;TrustServerCertificate=True"));
            var serviceProvider = services.BuildServiceProvider();
            _context = serviceProvider.GetRequiredService<AppDbContext>();
        }

        private async Task RunInTransactionAsync(Func<Task> testAction)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await testAction();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await transaction.RollbackAsync();
            }
        }

        [Fact]
        public async Task GetTaskItems_ReturnsAllTasks()
        {
            await RunInTransactionAsync(async () =>
            {
                // Arrange
                var controller = new TasksController(_context);

                // Act
                var result = await controller.GetTaskItems();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var taskDtos = Assert.IsType<List<TaskDTO>>(okResult.Value);
                Assert.True(taskDtos.Count >= 0); // Verifica se as tasks foram retornadas
            });
        }

        [Fact]
        public async Task GetTaskItemById_ReturnsTask_WhenTaskExists()
        {
            await RunInTransactionAsync(async () =>
            {
                // Arrange
                var controller = new TasksController(_context);

                var newTask = new TaskItem
                {
                    TicketNumber = "T123",
                    Title = "Test Task",
                    Description = "Test Description",
                    IsCompleted = false,
                    DueDate = DateTime.Now.AddDays(7),
                    Priority = "Alta",
                    AssigneId = 1, 
                    ProjectId = 1 
                };

                _context.TaskItems.Add(newTask);
                await _context.SaveChangesAsync();

                var taskId = newTask.Id;

                // Act
                var result = await controller.GetTaskItemById(taskId);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var taskDto = Assert.IsType<TaskDTO>(okResult.Value);
                Assert.Equal(newTask.Title, taskDto.Title);
            });
        }

        [Fact]
        public async Task CreateTaskItem_ReturnsCreatedTask_WhenValidDataIsProvided()
        {
            await RunInTransactionAsync(async () =>
            {
                // Arrange
                var controller = new TasksController(_context);

                var taskDto = new TaskDTO
                {
                    TicketNumber = "T456",
                    Title = "New Task",
                    Description = "New Task Description",
                    IsCompleted = false,
                    DueDate = DateTime.Now.AddDays(14),
                    Priority = "Média",
                    AssigneId = 1, 
                    ProjectId = 1 
                };

                // Act
                var result = await controller.CreateTaskItem(taskDto);

                // Assert
                var actionResult = Assert.IsType<CreatedAtActionResult>(result);
                Assert.Equal(nameof(controller.GetTaskItemById), actionResult.ActionName);

                var createdTask = Assert.IsType<TaskItem>(actionResult.Value);
                Assert.Equal(taskDto.Title, createdTask.Title);
            });
        }

        [Fact]
        public async Task UpdateTaskItem_ReturnsNoContent_WhenTaskIsUpdatedSuccessfully()
        {
            await RunInTransactionAsync(async () =>
            {
                // Arrange
                var controller = new TasksController(_context);

                var newTask = new TaskItem
                {
                    TicketNumber = "T789",
                    Title = "Old Task",
                    Description = "Old Task Description",
                    IsCompleted = false,
                    DueDate = DateTime.Now.AddDays(10),
                    Priority = "Baixa",
                    AssigneId = 1, 
                    ProjectId = 1 
                };

                _context.TaskItems.Add(newTask);
                await _context.SaveChangesAsync();

                var taskId = newTask.Id;

                var updatedTaskDto = new TaskDTO
                {
                    TicketNumber = "T789",
                    Title = "Updated Task",
                    Description = "Updated Task Description",
                    IsCompleted = true,
                    DueDate = DateTime.Now.AddDays(20),
                    Priority = "Alta",
                    AssigneId = 1,
                    ProjectId = 1
                };

                // Act
                var result = await controller.UpdateTaskItem(taskId, updatedTaskDto);

                // Assert
                Assert.IsType<NoContentResult>(result);

                var updatedTask = await _context.TaskItems.FindAsync(taskId);
                Assert.Equal(updatedTaskDto.Title, updatedTask.Title);
            });
        }

        [Fact]
        public async Task DeleteTaskItem_ReturnsNoContent_WhenTaskIsDeletedSuccessfully()
        {
            await RunInTransactionAsync(async () =>
            {
                // Arrange
                var controller = new TasksController(_context);

                var newTask = new TaskItem
                {
                    TicketNumber = "T999",
                    Title = "Task to Delete",
                    Description = "Task Description",
                    IsCompleted = false,
                    DueDate = DateTime.Now.AddDays(5),
                    Priority = "Alta",
                    AssigneId = 1, 
                    ProjectId = 1 
                };

                _context.TaskItems.Add(newTask);
                await _context.SaveChangesAsync();

                var taskId = newTask.Id;

                // Act
                var result = await controller.DeleteTaskItem(taskId);

                // Assert
                Assert.IsType<NoContentResult>(result);

                var deletedTask = await _context.TaskItems.FindAsync(taskId);
                Assert.Null(deletedTask);
            });
        }
    }
}
