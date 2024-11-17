using Microsoft.AspNetCore.Mvc;
using TMS.Controller; // Namespace correto do controlador
using TMS.Models;     // Namespace correto dos modelos
using Xunit;

namespace ProjetoTestes
{
    public class ProjectControllerTests 
    {
        private readonly ProjectController ProjController;

        public ProjectControllerTests() 
        {
            ProjController = new ProjectController();
        }


        //Teste primeiro ENDPOINT
        [Fact]
        public void GetProjects_ShouldReturnAllProjects()
        {
            // Act
            var result = ProjController.GetProjects();

            // Assert
            Assert.NotNull(result.Value); // Certifique-se de que a lista de projetos não é nula
            var firstProject = result.Value[0]; // Obtém o primeiro projeto
            Assert.Equal("Project 1", firstProject.Name);
        }


        //Teste Segundo ENDPOINT
        [Fact]
        public void GetProject_ExistingId_ReturnsProject()
        {
            // Act
            var result = ProjController.GetProject(3);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Project>>(result); // Este comando verifica se o resultado é do tipo ActionResult<Project> (Teste 1)
            var project = Assert.IsType<Project>(actionResult.Value); // Verifica se o valor do ActionResult é do tipo Project (Teste 2)
            Assert.Equal(3, project.Id); // Verifica se o ID do projeto retorna o mesmo valor que colocamos na var Result (Teste 3)
            Assert.Equal("Project 3", project.Name); //Verifica se o nome do projeto retornado é "Project 3" (Teste 4)
        }

        [Fact]
        public void GetProject_NonExistingId_ReturnsNotFound()
        {
            // Act
            var result = ProjController.GetProject(99);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Project>>(result); //(Teste 1)
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result); //Este comando verifica se o resultado é do tipo NotFoundObjectResult (Teste 2)
            Assert.Equal("Error: Project not found!", notFoundResult.Value); //Valida e mostra a mensagem de erro.
        }


        //Teste Terceiro ENDPOINT

        [Fact]
        public void CreateProject_ValidProject_ReturnsCreatedResult()
        {
            // Arrange
            var newProject = new Project
            {
                Id = 6,
                Name = "Project 6",
                Description = "Novo Projeto de Teste",
                StartDate = DateTime.Parse("2024-03-09"),
                EndDate = DateTime.Parse("2024-10-15"),
                Tasks = new List<TaskItem> {
            new TaskItem
            {
                Id = 1,
                TicketNumber = "003",
                Title = "Nova Task",
                Description = "Descrição da nova tarefa",
                IsCompleted = false,
                DueDate = DateTime.Now.AddDays(7),
                Priority = "Medium",
                Assigne = new User
                {
                    Id = 3,
                    UserName = "user3",
                    Email = "3@x.com",
                    FullName = "User 3",
                    Role = "Tester"
                }
            }
        }
            };

            // Act
            var result = ProjController.CreateProject(newProject);

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Project>>>(result);
            var createdResult = Assert.IsType<CreatedResult>(actionResult.Result);
            Assert.Equal("Project created!", createdResult.Location);
            var projects = Assert.IsType<List<Project>>(createdResult.Value);

            var projectFounded = ProjController.GetProject(6);
            Assert.Equal(newProject.Id, projectFounded.Value.Id);
            Assert.Equal(newProject.Name, projectFounded.Value.Name);
        }



        //Teste Quarto ENDPOINT
        [Fact]
        public void UpdateProject_ExistingId_UpdatesProject()
        {
            // Arrange
            var existingProject = ProjController.GetProjects().Value.FirstOrDefault(); //Seleciona o primeiro ou se não existir , retorna nulo.
            if (existingProject == null)
                throw new InvalidOperationException("Nenhum projeto existente foi encontrado para o teste."); //(Teste 1)

            var updatedProject = new Project
            {
                Id = existingProject.Id, // Reutiliza o ID de um projeto existente
                Name = "Updated Project",
                Description = "Descrição atualizada",
                StartDate = DateTime.Parse("2024-03-09"),
                EndDate = DateTime.Parse("2024-08-20"),
                Tasks = existingProject.Tasks // Reutiliza as tarefas existentes
            };

            // Act
            var result = ProjController.UpdateProject(updatedProject);

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Project>>>(result); //(Teste 2)
            var updatedProjects = Assert.IsType<List<Project>>(actionResult.Value); //(Teste 3)

            var modifiedProject = updatedProjects.FirstOrDefault();
            Assert.NotNull(modifiedProject); //Verifica se o projeto foi modificado, não sendo nulo. (Teste 4)
            Assert.Equal("Updated Project", modifiedProject.Name); //(Teste 5)
            Assert.Equal("Descrição atualizada", modifiedProject.Description); //(Teste 6)
            Assert.Equal(DateTime.Parse("2024-03-09"), modifiedProject.StartDate); //(Teste 7)
            Assert.Equal(DateTime.Parse("2024-08-20"), modifiedProject.EndDate); //(Teste 8)
        }


        //Teste Quinto ENDPOINT

        [Fact]
        public void DeleteProject_RemovesProject()
        {
            // Arrange

            var projectToDelete = ProjController.GetProject(2); // Busca o projeto com ID 2
            if (projectToDelete == null)
                throw new InvalidOperationException("Nenhum projeto existente foi encontrado para o teste."); // Lança exceção se não houver projeto com ID 2 (Teste 1)

            var allInitialProjectsCount = ProjController.GetProjects().Value.Count; // Obtém a contagem dos projetos antes do delete
            
            // Act
            var result = ProjController.DeleteProject(projectToDelete.Value.Id); // Chama o método DeleteProject

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Project>>>(result); // Garante que o resultado é do tipo ActionResult<List<Project>> (Teste 2)
            var updatedProjects = Assert.IsType<List<Project>>(actionResult.Value); // Garante que o Value do resultado é uma lista de projetos (Teste 3)

            // Verifica que o projeto foi removido da lista retornada
            Assert.DoesNotContain(updatedProjects, p => p.Id == projectToDelete.Value.Id); //(Teste 4)

            // Verifica que o tamanho da lista retornada foi reduzido em 1
            Assert.Equal(updatedProjects.Count, allInitialProjectsCount -1); //(Teste 5)

        }

        [Fact]
        public void DeleteProject_ReturnsNotFound()
        {
            // Arrange
            var invalidId = -1; // Um ID que não existe na lista

            // Act
            var result = ProjController.DeleteProject(invalidId);

            // Assert
            var actionResult = Assert.IsType<NotFoundObjectResult>(result.Result); // Confirma que é retornado NotFound
            var message = Assert.IsType<string>(actionResult.Value); // Verifica o conteúdo da mensagem
            Assert.Equal("Error: Project not found!", message);
        }


    }
}
