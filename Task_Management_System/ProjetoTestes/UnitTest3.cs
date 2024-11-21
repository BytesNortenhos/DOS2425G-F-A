using Microsoft.AspNetCore.Mvc;
using TMS.Controller;
using TMS.Models;
using Xunit;

namespace ProjetoTestes
{
    public class CommentControllerTests
    {
        private readonly CommentsController CommentsController;

        public CommentControllerTests()
        {
            CommentsController = new CommentsController();
        }


        //Teste primeiro ENDPOINT
        [Fact]
        public void GetComments_ShouldReturnAllComments()
        {
            // Act
            var result = CommentsController.GetComments();

            // Assert
            Assert.NotNull(result.Value);
            var firstComment = result.Value[0];
            Assert.Equal("comment1", firstComment.Text);
        }


        //Teste Segundo ENDPOINT
        [Fact]
        public void GetComment_ExistingId_ReturnsComment()
        {
            // Act
            var result = CommentsController.GetComment(3);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Comments>>(result);
            var comment = Assert.IsType<Comments>(actionResult.Value);
            Assert.Equal(3, comment.Id);
            Assert.Equal("user3", comment.Text);
        }

        [Fact]
        public void GetComment_NonExistingId_ReturnsNotFound()
        {
            // Act
            var result = CommentsController.GetComment(99);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Comments>>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Equal("Error: Comment not found!", notFoundResult.Value);
        }


        //Teste Terceiro ENDPOINT

        [Fact]
        public void CreateComment_ValidComment_ReturnsCreatedResult()
        {
            // Arrange
            var newComment = new Comments { Id = 6, Text = "FFF", User = new User { Id = 6, UserName = "user6", Email = "6@x.com", FullName = "User 6", Role = "Admin", tickets = new List<TaskItem> { new TaskItem { Id = 6, TicketNumber = "006", Title = "Task 6", Description = "Fff", IsCompleted = false, DueDate = DateTime.Now.AddDays(1), Priority = "High", Assigne = new User { Id = 6, UserName = "user6", Email = "6@x.com", FullName = "User 6", Role = "Admin" } }, new TaskItem { Id = 6, TicketNumber = "006", Title = "Task 6", Description = "Fff", IsCompleted = false, DueDate = DateTime.Now.AddDays(2), Priority = "Low", Assigne = new User { Id = 6, UserName = "user6", Email = "6@x.com", FullName = "User 6", Role = "Dev" } } } }, TaskId = new TaskItem { Id = 6, TicketNumber = "006", Title = "Tarefa 6", Description = "Fff", IsCompleted = false, DueDate = new DateTime(2023, 10, 9) } };
            // Act
            var result = CommentsController.CreateComment(newComment);

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Comments>>>(result);
            var createdResult = Assert.IsType<CreatedResult>(actionResult.Result);
            Assert.Equal("Comment created!", createdResult.Location);
            var comments = Assert.IsType<List<Comments>>(createdResult.Value);

            var commentFounded = CommentsController.GetComment(6);
            Assert.Equal(newComment.Id, commentFounded.Value.Id);
            Assert.Equal(newComment.Text, commentFounded.Value.Text);
        }



        //Teste Quarto ENDPOINT
        [Fact]
        public void UpdateComment_ExistingId_UpdatesComment()
        {
            // Arrange
            var existingComment = CommentsController.GetComments().Value.FirstOrDefault();
            if (existingComment == null)
                throw new InvalidOperationException("Nenhum comment existente foi encontrado para o teste.");

            var updatedComment = new Comments
            {
                Id = existingComment.Id,
                Text = "Updated Comment",
                User = existingComment.User,
                TaskId = existingComment.TaskId,
            };

            // Act
            var result = CommentsController.UpdateComment(updatedComment);

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Comments>>>(result);
            var updatedComments = Assert.IsType<List<Comments>>(actionResult.Value);

            var modifiedComment = updatedComments.FirstOrDefault();
            Assert.NotNull(modifiedComment);
            Assert.Equal(existingComment.Id, modifiedComment.Id);
            Assert.Equal("Updated Comment", modifiedComment.Text);
            Assert.Equal(existingComment.User, modifiedComment.User);
            Assert.Equal(existingComment.TaskId, modifiedComment.TaskId);
        }


        //Teste Quinto ENDPOINT

        [Fact]
        public void DeleteComment_RemovesComment()
        {
            // Arrange

            var commentToDelete = CommentsController.GetComment(2);
            if (commentToDelete == null)
                throw new InvalidOperationException("Nenhum comment existente foi encontrado para o teste.");

            var allInitialUsersCount = CommentsController.GetComments().Value.Count;

            // Act
            var result = CommentsController.DeleteComment(commentToDelete.Value.Id);

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Comments>>>(result);
            var updatedComments = Assert.IsType<List<Comments>>(actionResult.Value);

            // Verifica que o user foi removido da lista retornada
            Assert.DoesNotContain(updatedComments, p => p.Id == commentToDelete.Value.Id);

            // Verifica que o tamanho da lista retornada foi reduzido em 1
            Assert.Equal(updatedComments.Count, allInitialUsersCount - 1);

        }

        [Fact]
        public void DeleteComment_ReturnsNotFound()
        {
            // Arrange
            var invalidId = -1; // Um ID que não existe na lista

            // Act
            var result = CommentsController.DeleteComment(invalidId);

            // Assert
            var actionResult = Assert.IsType<NotFoundObjectResult>(result.Result); // Confirma que é retornado NotFound
            var message = Assert.IsType<string>(actionResult.Value); // Verifica o conteúdo da mensagem
            Assert.Equal("Error: Comment not found!", message);
        }


    }
}