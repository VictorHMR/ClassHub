using Moq;
using Xunit;
using ClassHub.Controllers;
using ClassHub.ClassHubContext.Services;
using ClassHub.Dtos.Nota;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassHub.Tests
{
    public class NotaControllerTests
    {
        private readonly Mock<NotaService> _mockNotaService;
        private readonly NotaController _notaController;

        public NotaControllerTests()
        {
            _mockNotaService = new Mock<NotaService>(null);
            _notaController = new NotaController(_mockNotaService.Object);
        }

        [Fact]
        public async Task ListarNotasAluno_DeveRetornarNotas_QuandoNotasExistirem()
        {
            // Arrange
            var alunoTurmaId = 1;
            var notas = new List<ListarNotasAlunoResponseDTO>
            {
                new ListarNotasAlunoResponseDTO { IdNota = 1, Nota = 7.0, Descricao = "Prova 1" },
                new ListarNotasAlunoResponseDTO { IdNota = 2, Nota = 8.5, Descricao = "Prova 2" }
            };

            _mockNotaService.Setup(service => service.ListarNotasAluno(alunoTurmaId))
                .ReturnsAsync(notas);

            // Act
            var result = await _notaController.ListarNotasAluno(alunoTurmaId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var resultNotas = result.Value as List<ListarNotasAlunoResponseDTO>;
            Assert.Equal(2, resultNotas.Count);
            Assert.Contains(resultNotas, n => n.Nota == 7.0);
            Assert.Contains(resultNotas, n => n.Nota == 8.5);
        }

        [Fact]
        public async Task Lancar_DeveInserirNota_QuandoDadosEstiveremCorretos()
        {
            // Arrange
            var dto = new LancarNotaRequestDTO
            {
                IdAlunoTurma = 1,
                Nota = 9.0,
                Descricao = "Prova Final"
            };

            _mockNotaService.Setup(service => service.InserirNotaAsync(dto))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _notaController.Lancar(dto) as OkResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            _mockNotaService.Verify(service => service.InserirNotaAsync(dto), Times.Once);
        }

        [Fact]
        public async Task Editar_DeveEditarNota_QuandoNotaExistir()
        {
            // Arrange
            var dto = new EditarNotaRequestDTO
            {
                IdNota = 1,
                Nota = 9.5,
                Descricao = "Prova Final"
            };

            _mockNotaService.Setup(service => service.EditarNotaAsync(dto))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _notaController.Editar(dto) as OkResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            _mockNotaService.Verify(service => service.EditarNotaAsync(dto), Times.Once);
        }


        [Fact]
        public async Task Deletar_DeveDeletarNota_QuandoNotaExistir()
        {
            // Arrange
            var idNota = 1;

            _mockNotaService.Setup(service => service.DeletarNotaAsync(idNota))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _notaController.Deletar(idNota) as OkResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            _mockNotaService.Verify(service => service.DeletarNotaAsync(idNota), Times.Once);
        }

        [Fact]
        public async Task Lancar_DeveRetornarBadRequest_QuandoErro()
        {
            // Arrange
            var dto = new LancarNotaRequestDTO
            {
                IdAlunoTurma = 1,
                Nota = 9.0,
                Descricao = "Prova Final"
            };

            _mockNotaService.Setup(service => service.InserirNotaAsync(dto))
                .ThrowsAsync(new System.Exception("Erro ao inserir nota"));

            // Act
            var result = await _notaController.Lancar(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }
    }
}