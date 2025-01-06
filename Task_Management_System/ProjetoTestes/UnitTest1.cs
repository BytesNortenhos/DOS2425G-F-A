using Microsoft.AspNetCore.Mvc;
using TMS.Controller; 
using TMS.Models;     
using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;


namespace ProjetoTestes
{
    public class ProjectControllerTests
    {
        private readonly AppDbContext _context;

        public ProjectControllerTests()
        {
            var services = new ServiceCollection();
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer("Server=JCOSTA_ASUS\\SQLEXPRESS;Database=DosBD;User Id=sa;Password=PasswordTeste123!;TrustServerCertificate=True"));
            var serviceProvider = services.BuildServiceProvider();
            _context = serviceProvider.GetRequiredService<AppDbContext>();
        }


        private async Task RunInTransactionAsync(Func<Task> testAction)
        {
            // Inicia uma nova transação
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Executa o teste
                await testAction();
            }
            catch
            {
                // Se ocorrer algum erro, realiza o rollback da transação
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                // Garante que a transação seja revertida após o teste (mesmo em sucesso), assim garante que nada é guardado na base de dados
                await transaction.RollbackAsync(); 
            }
        }

        [Fact]
        public async Task GetProjects_ReturnsAllProjects()
        {
            // Transação para garantir que os dados não sejam persistidos após o teste
            await RunInTransactionAsync(async () =>
            {
                // Arrange
                var controller = new ProjectController(_context);

                // Act
                var result = await controller.GetProjects();

                // Assert
                var actionResult = Assert.IsType<ActionResult<IEnumerable<Project>>>(result);
                var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
                var projects = Assert.IsType<List<Project>>(okResult.Value);
                Assert.True(projects.Count > 0); // Verifica se ao menos um projeto foi retornado
            });
        }

        [Fact]
        public async Task GetProjectById_ReturnsProject_WhenProjectExists()
        {
            await RunInTransactionAsync(async () =>
            {
                // Arrange
                var controller = new ProjectController(_context);

                var newProject = new Project
                {
                    ProjectName = "Projeto Teste 6",
                    Description = "Descrição do projeto teste 6",
                    StartDate = DateTime.Parse("2024-08-22", CultureInfo.InvariantCulture),
                    EndDate = DateTime.Parse("2024-10-22", CultureInfo.InvariantCulture)
                };

                _context.Projects.Add(newProject);
                await _context.SaveChangesAsync();  // Este `SaveChangesAsync` não persistirá os dados devido ao rollback

                // Obter o id do projeto recém-criado
                var projectId = newProject.Id;

                // Act
                var result = await controller.GetProjectById(projectId);

                // Assert
                var actionResult = Assert.IsType<ActionResult<Project>>(result);
                var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
                var project = Assert.IsType<Project>(okResult.Value);
                Assert.Equal(newProject.ProjectName, project.ProjectName);
                Assert.Equal(newProject.Description, project.Description);
            });
        }

        [Fact]
        public async Task CreateProject_ReturnsCreatedProject_WhenValidProjectDTOIsProvided()
        {
            await RunInTransactionAsync(async () =>
            {
                // Arrange
                var controller = new ProjectController(_context);

                var project = new ProjectDTO
                {
                    ProjectName = "Projeto kakakak",
                    Description = "Descrição do projeto kakakakaka",
                    StartDate = DateTime.Parse("2024-01-01", CultureInfo.InvariantCulture),
                    EndDate = DateTime.Parse("2024-12-31", CultureInfo.InvariantCulture)
                };

                // Act
                var result = await controller.CreateProject(project);

                
                // Assert
                var actionResult = Assert.IsType<CreatedAtActionResult>(result);
                Assert.Equal(nameof(controller.GetProjectById), actionResult.ActionName);

                var createdProject = Assert.IsType<Project>(actionResult.Value);
                Assert.Equal(project.ProjectName, createdProject.ProjectName);
                Assert.Equal(project.Description, createdProject.Description);
                Assert.Equal(project.StartDate, createdProject.StartDate);
                Assert.Equal(project.EndDate, createdProject.EndDate);

                var savedProject = await _context.Projects.FindAsync(createdProject.Id);
                Assert.NotNull(savedProject);
                Assert.Equal(project.ProjectName, savedProject.ProjectName);
            });
        }

        [Fact]
        public async Task UpdateProject_ReturnsNoContent_WhenProjectIsUpdatedSuccessfully()
        {
            await RunInTransactionAsync(async () =>
            {
                // Arrange
                var controller = new ProjectController(_context);

                var newProject = new Project
                {
                    ProjectName = "Projeto Original",
                    Description = "Descrição Original",
                    StartDate = DateTime.Parse("2024-01-01", CultureInfo.InvariantCulture),
                    EndDate = DateTime.Parse("2024-12-31", CultureInfo.InvariantCulture)
                };

                _context.Projects.Add(newProject);
                await _context.SaveChangesAsync();

                var projectId = newProject.Id;

                var updatedProjectDTO = new ProjectDTO
                {
                    ProjectName = "Projeto Atualizado",
                    Description = "Descrição Atualizada",
                    StartDate = DateTime.Parse("2025-01-01", CultureInfo.InvariantCulture),
                    EndDate = DateTime.Parse("2025-12-31", CultureInfo.InvariantCulture)
                };

                // Act
                var result = await controller.UpdateProject(projectId, updatedProjectDTO);

                // Assert
                Assert.IsType<NoContentResult>(result); // Verifica se o resultado é NoContent (204)

                var updatedProject = await _context.Projects.FindAsync(projectId);
                Assert.NotNull(updatedProject); // Garante que o projeto ainda existe
                Assert.Equal(updatedProjectDTO.ProjectName, updatedProject.ProjectName);
                Assert.Equal(updatedProjectDTO.Description, updatedProject.Description);
                Assert.Equal(updatedProjectDTO.StartDate, updatedProject.StartDate);
                Assert.Equal(updatedProjectDTO.EndDate, updatedProject.EndDate);
            });
        }

        [Fact]
        public async Task DeleteProject_ReturnsNoContent_WhenProjectIsDeletedSuccessfully()
        {
            await RunInTransactionAsync(async () =>
            {
                // Arrange
                var controller = new ProjectController(_context);

                var newProject = new Project
                {
                    ProjectName = "Projeto para Deletar",
                    Description = "Descrição do projeto para deletar",
                    StartDate = DateTime.Parse("2024-01-01", CultureInfo.InvariantCulture),
                    EndDate = DateTime.Parse("2024-12-31", CultureInfo.InvariantCulture)
                };

                _context.Projects.Add(newProject);
                await _context.SaveChangesAsync();

                var projectId = newProject.Id;

                // Act
                var result = await controller.DeleteProject(projectId);

                // Assert
                Assert.IsType<NoContentResult>(result); // Verifica se o retorno é NoContent (204)

                var deletedProject = await _context.Projects.FindAsync(projectId);
                Assert.Null(deletedProject);
            });
        }

    }
}
