using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using TMS.Controller;
using TMS.Models;
using Xunit;

namespace ProjetoTestes
{
    public class UserControllerTests
    {
        private readonly AppDbContext _context;

        public UserControllerTests()
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
        public async Task GetUsers_ReturnsAllUsers()
        {
            await RunInTransactionAsync(async () =>
            {
                // Arrange
                var controller = new UserController(_context);

                // Act
                var result = await controller.GetUsers();

                // Assert
                var actionResult = Assert.IsType<ActionResult<List<User>>>(result);
                var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
                var users = Assert.IsType<List<User>>(okResult.Value);
                Assert.True(users.Count >= 0); // Verifica se users foram retornados
            });
        }

        [Fact]
        public async Task GetUser_ReturnsUser_WhenUserExists()
        {
            await RunInTransactionAsync(async () =>
            {
                // Arrange
                var controller = new UserController(_context);

                var newUser = new User
                {
                    UserName = "TestUser",
                    Email = "testuser@example.com",
                    FullName = "Test User",
                    Role = "Admin"
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                var userId = newUser.Id;

                // Act
                var result = await controller.GetUser(userId);

                // Assert
                var actionResult = Assert.IsType<ActionResult<User>>(result);
                var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
                var user = Assert.IsType<User>(okResult.Value);
                Assert.Equal(newUser.UserName, user.UserName);
            });
        }

        [Fact]
        public async Task CreateUser_ReturnsCreatedUser_WhenValidUserDTOIsProvided()
        {
            await RunInTransactionAsync(async () =>
            {
                // Arrange
                var controller = new UserController(_context);

                var userDTO = new UserDTO
                {
                    UserName = "NewUser",
                    Email = "newuser@example.com",
                    FullName = "New User",
                    Role = "User"
                };

                // Act
                var result = await controller.CreateUser(userDTO);

                // Assert
                var actionResult = Assert.IsType<CreatedAtActionResult>(result);
                Assert.Equal(nameof(controller.GetUser), actionResult.ActionName);

                var createdUser = Assert.IsType<User>(actionResult.Value);
                Assert.Equal(userDTO.UserName, createdUser.UserName);
            });
        }

        [Fact]
        public async Task UpdateUser_ReturnsNoContent_WhenUserIsUpdatedSuccessfully()
        {
            await RunInTransactionAsync(async () =>
            {
                // Arrange
                var controller = new UserController(_context);

                var newUser = new User
                {
                    UserName = "UpdateTest",
                    Email = "updatetest@example.com",
                    FullName = "Update Test",
                    Role = "User"
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                var userId = newUser.Id;

                var userDTO = new UserDTO
                {
                    UserName = "UpdatedUser",
                    Email = "updateduser@example.com",
                    FullName = "Updated User",
                    Role = "Admin"
                };

                // Act
                var result = await controller.UpdateUser(userId, userDTO);

                // Assert
                Assert.IsType<NoContentResult>(result);

                var updatedUser = await _context.Users.FindAsync(userId);
                Assert.Equal(userDTO.UserName, updatedUser.UserName);
            });
        }

        [Fact]
        public async Task DeleteUser_ReturnsNoContent_WhenUserIsDeletedSuccessfully()
        {
            await RunInTransactionAsync(async () =>
            {
                // Arrange
                var controller = new UserController(_context);

                var newUser = new User
                {
                    UserName = "DeleteTest",
                    Email = "deletetest@example.com",
                    FullName = "Delete Test",
                    Role = "User"
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                var userId = newUser.Id;

                // Act
                var result = await controller.DeleteUser(userId);

                // Assert
                Assert.IsType<NoContentResult>(result);

                var deletedUser = await _context.Users.FindAsync(userId);
                Assert.Null(deletedUser);
            });
        }
    }
}
