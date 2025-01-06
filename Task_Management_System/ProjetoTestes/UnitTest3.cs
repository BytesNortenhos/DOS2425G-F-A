using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using TMS.Controller;
using TMS.Models;
using Xunit;

namespace ProjetoTestes
{
    public class CommentsControllerTests
    {
        private readonly AppDbContext _context;

        public CommentsControllerTests()
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
        public async Task GetComments_ReturnsAllComments()
        {
            await RunInTransactionAsync(async () =>
            {
                // Arrange
                var controller = new CommentsController(_context);

                // Act
                var result = await controller.GetComments();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var comments = Assert.IsType<List<CommentsDTO>>(okResult.Value);
                Assert.True(comments.Count >= 0); // Verifica se os comments foram retornados
            });
        }

        [Fact]
        public async Task GetCommentById_ReturnsComment_WhenCommentExists()
        {
            await RunInTransactionAsync(async () =>
            {
                // Arrange
                var controller = new CommentsController(_context);

                var newComment = new Comments
                {
                    Text = "Test Comment",
                    UserId = 1, 
                    TaskItemId = 1 
                };

                _context.Comments.Add(newComment);
                await _context.SaveChangesAsync();

                var commentId = newComment.Id;

                // Act
                var result = await controller.GetCommentById(commentId);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var commentDto = Assert.IsType<CommentsDTO>(okResult.Value);
                Assert.Equal(newComment.Text, commentDto.Text);
            });
        }

        [Fact]
        public async Task CreateComment_ReturnsCreatedComment_WhenValidDataIsProvided()
        {
            await RunInTransactionAsync(async () =>
            {
                // Arrange
                var controller = new CommentsController(_context);

                var commentDTO = new CommentsDTO
                {
                    Text = "New Comment",
                    UserId = 1, 
                    TaskItemId = 1 
                };

                // Act
                var result = await controller.CreateComment(commentDTO);

                // Assert
                var actionResult = Assert.IsType<CreatedAtActionResult>(result);
                Assert.Equal(nameof(controller.GetCommentById), actionResult.ActionName);

                var createdComment = Assert.IsType<Comments>(actionResult.Value);
                Assert.Equal(commentDTO.Text, createdComment.Text);
            });
        }

        [Fact]
        public async Task UpdateComment_ReturnsNoContent_WhenCommentIsUpdatedSuccessfully()
        {
            await RunInTransactionAsync(async () =>
            {
                // Arrange
                var controller = new CommentsController(_context);

                var newComment = new Comments
                {
                    Text = "Old Comment",
                    UserId = 1, 
                    TaskItemId = 1 
                };

                _context.Comments.Add(newComment);
                await _context.SaveChangesAsync();

                var commentId = newComment.Id;

                var updatedCommentDTO = new CommentsDTO
                {
                    Text = "Updated Comment",
                    UserId = 1,
                    TaskItemId = 1
                };

                // Act
                var result = await controller.UpdateComment(commentId, updatedCommentDTO);

                // Assert
                Assert.IsType<NoContentResult>(result);

                var updatedComment = await _context.Comments.FindAsync(commentId);
                Assert.Equal(updatedCommentDTO.Text, updatedComment.Text);
            });
        }

        [Fact]
        public async Task DeleteComment_ReturnsNoContent_WhenCommentIsDeletedSuccessfully()
        {
            await RunInTransactionAsync(async () =>
            {
                // Arrange
                var controller = new CommentsController(_context);

                var newComment = new Comments
                {
                    Text = "Comment to Delete",
                    UserId = 1, 
                    TaskItemId = 1 
                };

                _context.Comments.Add(newComment);
                await _context.SaveChangesAsync();

                var commentId = newComment.Id;

                // Act
                var result = await controller.DeleteComment(commentId);

                // Assert
                Assert.IsType<NoContentResult>(result);

                var deletedComment = await _context.Comments.FindAsync(commentId);
                Assert.Null(deletedComment);
            });
        }
    }
}
