using ClassHub.ClassHubContext.Models;
using ClassHub.ClassHubContext.Services;
using ClassHub.Controllers;
using ClassHub.Dtos.Turma;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ClassHub.Tests
{
    public class TurmaControllerTests
    {
        private readonly Mock<TurmaService> _mockTurmaService;
        private readonly TurmaController _controller;

        public TurmaControllerTests()
        {
            _mockTurmaService = new Mock<TurmaService>(null, null);
            _controller = new TurmaController(_mockTurmaService.Object);
        }

        [Fact]
        public async Task Listar_DeveRetornarOk()
        {
            // Arrange
            var request = new ListarTurmaRequestDTO { nrPagina = 1, qtRegistros = 10 };
            var resultMock = new PaginacaoResult<TurmaDTO>
            {
                Itens = new[] { new TurmaDTO { IdTurma = 1, Nome = "Turma A" } },
                TotalItens = 1,
                TotalPaginas = 1,
                PaginaAtual = 1,
                TamanhoPagina = 10
            };
            _mockTurmaService.Setup(service => service.ListarTurmasAsync(request)).ReturnsAsync(resultMock);

            // Act
            var result = await _controller.Listar(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PaginacaoResult<TurmaDTO>>(okResult.Value);
            Assert.Equal(1, returnValue.TotalItens);
        }

        [Fact]
        public async Task ObterTurma_DeveRetornarOk()
        {
            // Arrange
            var turmaId = 1;
            var resultMock = new TurmaDTO
            {
                IdTurma = turmaId,
                Nome = "Turma Teste",
                NomeProfessor = "Professor Teste"
            };
            _mockTurmaService.Setup(service => service.ObterTurmaPorId(turmaId)).ReturnsAsync(resultMock);

            // Act
            var result = await _controller.ObterTurma(turmaId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<TurmaDTO>(okResult.Value);
            Assert.Equal("Turma Teste", returnValue.Nome);
            Assert.Equal("Professor Teste", returnValue.NomeProfessor);
        }

        [Fact]
        public async Task Criar_DeveRetornarOk()
        {
            // Arrange
            var request = new CriarTurmaRequestDTO
            {
                Nome = "Turma Criada",
                IdProfessor = 1
            };
            _mockTurmaService.Setup(service => service.CriarTurmaAsync(request)).ReturnsAsync(1);

            // Act
            var result = await _controller.Criar(request);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Editar_DeveRetornarOk()
        {
            // Arrange
            var request = new EditarTurmaRequestDTO
            {
                IdTurma = 1,
                Nome = "Turma Editada",
                IdProfessor = 2,
                DtFim = DateTime.Now
            };
            _mockTurmaService.Setup(service => service.EditarTurmaAsync(request)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Editar(request);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Deletar_DeveRetornarOk()
        {
            // Arrange
            var turmaId = 1;
            _mockTurmaService.Setup(service => service.DeletarTurmaAsync(turmaId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Deletar(turmaId);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task VincularAluno_DeveRetornarOk()
        {
            // Arrange
            var request = new VincularAlunoTurmaRequestDTO
            {
                IdTurma = 1,
                RAAluno = "123",
                FlDesvincular = false
            };
            _mockTurmaService.Setup(service => service.VincularAlunoTurmaAsync(request)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.VincularAluno(request);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }
    }
}