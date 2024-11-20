using Microsoft.AspNetCore.Mvc;
using TMS.Controller; // Namespace correto do controlador
using TMS.Models;     // Namespace correto dos modelos
using Xunit;

namespace ProjetoTestes
{
    public class UserControllerTests 
    {
        private readonly UserController UserController;

        public UserControllerTests() 
        {
            UserController = new UserController();
        }


        //Teste primeiro ENDPOINT
        [Fact]
        public void GetUsers_ShouldReturnAllUsers()
        {
            // Act
            var result = UserController.GetUsers();

            // Assert
            Assert.NotNull(result.Value);
            var firstUser = result.Value[0];
            Assert.Equal("user1", firstUser.UserName);
        }


        //Teste Segundo ENDPOINT
        [Fact]
        public void GetUser_ExistingId_ReturnsUser()
        {
            // Act
            var result = UserController.GetUser(3);

            // Assert
            var actionResult = Assert.IsType<ActionResult<User>>(result);
            var user = Assert.IsType<User>(actionResult.Value);
            Assert.Equal(3, user.Id);
            Assert.Equal("user3", user.UserName);
        }

        [Fact]
        public void GetUser_NonExistingId_ReturnsNotFound()
        {
            // Act
            var result = UserController.GetUser(99);

            // Assert
            var actionResult = Assert.IsType<ActionResult<User>>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Equal("Error: User not found!", notFoundResult.Value);
        }


        //Teste Terceiro ENDPOINT

        [Fact]
        public void CreateUser_ValidUser_ReturnsCreatedResult()
        {
            // Arrange
            var newUser = new User { Id = 6, UserName = "user6", Email = "6@x.com", FullName = "User 5", Role = "User", tickets = new List<TaskItem> { new TaskItem { Id = 1, TicketNumber = "001", Title = "Task 1", Description = "Aaa", IsCompleted = false, DueDate = DateTime.Now.AddDays(1), Priority = "High", Assigne = new User { Id = 1, UserName = "user1", Email = "1@x.com", FullName = "User 1", Role = "Admin" } }, new TaskItem { Id = 2, TicketNumber = "002", Title = "Task 2", Description = "Aaa", IsCompleted = false, DueDate = DateTime.Now.AddDays(2), Priority = "Low", Assigne = new User { Id = 1, UserName = "user2", Email = "2@x.com", FullName = "User 1", Role = "Dev" } } } };

            // Act
            var result = UserController.CreateUser(newUser);

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<User>>>(result);
            var createdResult = Assert.IsType<CreatedResult>(actionResult.Result);
            Assert.Equal("User created!", createdResult.Location);
            var users = Assert.IsType<List<User>>(createdResult.Value);

            var userFounded = UserController.GetUser(6);
            Assert.Equal(newUser.Id, userFounded.Value.Id);
            Assert.Equal(newUser.UserName, userFounded.Value.UserName);
        }



        //Teste Quarto ENDPOINT
        [Fact]
        public void UpdateUser_ExistingId_UpdatesUser()
        {
            // Arrange
            var existingUser = UserController.GetUsers().Value.FirstOrDefault();
            if (existingUser == null)
                throw new InvalidOperationException("Nenhum user existente foi encontrado para o teste.");

            var updatedUser = new User
            {
                Id = existingUser.Id,
                UserName = "Updated User",
                Email = "Email atualizado",
                FullName = "User 11",
                Role = "Admin",
                tickets = existingUser.tickets,
            };

            // Act
            var result = UserController.UpdateUser(updatedUser);

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<User>>>(result);
            var updatedUsers = Assert.IsType<List<User>>(actionResult.Value);

            var modifiedUser = updatedUsers.FirstOrDefault();
            Assert.NotNull(modifiedUser);
            Assert.Equal("Updated User", modifiedUser.UserName);
            Assert.Equal("Email atualizado", modifiedUser.Email);
            Assert.Equal("User 11", modifiedUser.FullName);
            Assert.Equal("Admin", modifiedUser.Role);
        }


        //Teste Quinto ENDPOINT

        [Fact]
        public void DeleteUser_RemovesUser()
        {
            // Arrange

            var userToDelete = UserController.GetUser(2);
            if (userToDelete == null)
                throw new InvalidOperationException("Nenhum user existente foi encontrado para o teste.");

            var allInitialUsersCount = UserController.GetUsers().Value.Count;
            
            // Act
            var result = UserController.DeleteUser(userToDelete.Value.Id);

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<User>>>(result);
            var updatedUsers = Assert.IsType<List<User>>(actionResult.Value);

            // Verifica que o user foi removido da lista retornada
            Assert.DoesNotContain(updatedUsers, p => p.Id == userToDelete.Value.Id);

            // Verifica que o tamanho da lista retornada foi reduzido em 1
            Assert.Equal(updatedUsers.Count, allInitialUsersCount -1);

        }

        [Fact]
        public void DeleteUser_ReturnsNotFound()
        {
            // Arrange
            var invalidId = -1; // Um ID que não existe na lista

            // Act
            var result = UserController.DeleteUser(invalidId);

            // Assert
            var actionResult = Assert.IsType<NotFoundObjectResult>(result.Result); // Confirma que é retornado NotFound
            var message = Assert.IsType<string>(actionResult.Value); // Verifica o conteúdo da mensagem
            Assert.Equal("Error: User not found!", message);
        }


    }
}
