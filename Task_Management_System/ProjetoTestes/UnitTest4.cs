using Microsoft.AspNetCore.Mvc;
using TMS.Controller; // Namespace correto do controlador
using TMS.Models;     // Namespace correto dos modelos
using Xunit;

namespace ProjetoTestes
{
    public class TasksControllerTest 
    {
        private readonly TasksController TasksController;

        public TasksControllerTest() 
        {
            TasksController = new TasksController();
        }


        //Teste primeiro ENDPOINT
        [Fact]
        public void GetTasks_ShouldReturnAllTasks()
        {
            // Act
            var result = TasksController.GetTasks();

            // Assert
            Assert.NotNull(result.Value);
            var firstTask = result.Value[0];
            Assert.Equal("task1", firstTask.Title);
        }


        //Teste Segundo ENDPOINT
        [Fact]
        public void GetTask_ExistingId_ReturnsTask()
        {
            // Act
            var result = TasksController.GetTask(3);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Task>>(result);
            var task = Assert.IsType<TaskItem>(actionResult.Value);
            Assert.Equal(3, task.Id);
            Assert.Equal("003", task.TicketNumber);
        }

        [Fact]
        public void GetTask_NonExistingId_ReturnsNotFound()
        {
            // Act
            var result = TasksController.GetTask(99);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TaskItem>>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Equal("Error: Task not found!", notFoundResult.Value);
        }


        //Teste Terceiro ENDPOINT

        [Fact]
        public void CreateTask_ValidUser_ReturnsCreatedResult()
        {
            var newTask = new TaskItem
            {
                Id = 5, TicketNumber = "005", Title = "Task 5", Description = "Aaa", IsCompleted = false,
                DueDate = DateTime.Now.AddDays(5), Priority = "Low",
                Assigne = new User
                    { Id = 1, UserName = "user3", Email = "3@x.com", FullName = "User 1", Role = "Admin" },
                Comments = new List<Comments>
                {
                    new Comments
                    {
                        Id = 1, Text = "Aaa",
                        TaskId = new TaskItem
                        {
                            Id = 1, TicketNumber = "001", Title = "Tarefa 1", Description = "Aaa", IsCompleted = false,
                            DueDate = new DateTime(2023, 10, 5)
                        }
                    },
                    new Comments
                    {
                        Id = 2, Text = "Bbb",
                        TaskId = new TaskItem
                        {
                            Id = 2, TicketNumber = "002", Title = "Tarefa 2", Description = "Bbb", IsCompleted = false,
                            DueDate = new DateTime(2023, 10, 6)
                        }
                    }
                }
            };

            var result = TasksController.CreateTask(newTask);

            var actionResult = Assert.IsType<ActionResult<List<Task>>>(result);
            var createdResult = Assert.IsType<CreatedResult>(actionResult.Result);
            Assert.Equal("User created!", createdResult.Location);
            var tasks = Assert.IsType<List<Task>>(createdResult.Value);

            var taskFounded = TasksController.GetTask(6);
            Assert.Equal(newTask.Id, taskFounded.Value.Id);
            Assert.Equal(newTask.TicketNumber, taskFounded.Value.TicketNumber);
        }



        //Teste Quarto ENDPOINT
        [Fact]
        public void UpdateTask_ExistingId_UpdatesTask()
        {
            var existingTask = TasksController.GetTasks().Value.FirstOrDefault();
            if (existingTask == null)
                throw new InvalidOperationException("Nenhum task existente foi encontrado para o teste.");

            var updatedTask = new TaskItem()
            {
                Id = existingTask.Id,
                TicketNumber = "006",
                Title = "Task 5",
                Description = "aaa",
                IsCompleted = false
            };

            var result = TasksController.UpdateTask(updatedTask);

            var actionResult = Assert.IsType<ActionResult<List<Task>>>(result);
            var updatedTasks = Assert.IsType<List<TaskItem>>(actionResult.Value);

            var modifiedTask = updatedTasks.FirstOrDefault();
            Assert.NotNull(modifiedTask);
            Assert.Equal("Updated Ticket Name", modifiedTask.TicketNumber);
            Assert.Equal("Email atualizado", modifiedTask.Title);
            Assert.Equal("User 11", modifiedTask.Description);
            Assert.False(modifiedTask.IsCompleted);
        }


        //Teste Quinto ENDPOINT

        [Fact]
        public void DeleteTask_RemovesTask()
        {

            var taskToDelete = TasksController.GetTask(2);
            if (taskToDelete == null)
                throw new InvalidOperationException("Nenhuma task existente foi encontrado para o teste.");

            var allInitialTaskCount = TasksController.GetTasks().Value.Count;
            
            var result = TasksController.DeleteTask(taskToDelete.Value.Id);

            var actionResult = Assert.IsType<ActionResult<List<Task>>>(result);
            var updatedTasks = Assert.IsType<List<TaskItem>>(actionResult.Value);

            Assert.DoesNotContain(updatedTasks, p => p.Id == taskToDelete.Value.Id);

            Assert.Equal(updatedTasks.Count, allInitialTaskCount -1);

        }

        [Fact]
        public void DeleteUser_ReturnsNotFound()
        {
            // Arrange
            var invalidId = -1; 

            // Act
            var result = TasksController.DeleteTask(invalidId);

            // Assert
            var actionResult = Assert.IsType<NotFoundObjectResult>(result.Result); // Confirma que � retornado NotFound
            var message = Assert.IsType<string>(actionResult.Value); // Verifica o conte�do da mensagem
            Assert.Equal("Error: Task not found!", message);
        }


    }
}
