using ClassHub.ClassHubContext.Models;
using ClassHub.ClassHubContext.Services;
using ClassHub.Controllers;
using ClassHub.Dtos.Usuario;
using ClassHub.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace ClassHub.Tests
{
    public class UsuarioControllerTests
    {
        private readonly Mock<UsuarioService> _usuarioServiceMock;
        private readonly UsuarioController _controller;

        public UsuarioControllerTests()
        {
            _usuarioServiceMock = new Mock<UsuarioService>(null, null);
            _controller = new UsuarioController(_usuarioServiceMock.Object);
        }

        [Fact]
        public async Task Login_DeveRetornarOk_QuandoLoginValido()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO
            {
                Login = "aluno1@gmail.com",
                Senha = "Senha123"
            };

            var loginResponse = new LoginResponseDTO
            {
                IdUsuario = 1,
                Nome = "Aluno1",
                Email = "aluno1@gmail.com",
                Token = "valid-jwt-token"
            };

            _usuarioServiceMock.Setup(service => service.ObterUsuarioAsync(loginRequest))
                               .ReturnsAsync(loginResponse);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseValue = Assert.IsType<LoginResponseDTO>(okResult.Value);
            Assert.Equal("Aluno1", responseValue.Nome);
        }

        [Fact]
        public async Task Login_DeveRetornarUnauthorized_QuandoLoginInvalido()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO
            {
                Login = "inexistente@gmail.com",
                Senha = "SenhaErrada"
            };

            _usuarioServiceMock.Setup(service => service.ObterUsuarioAsync(loginRequest))
                               .ReturnsAsync((LoginResponseDTO)null);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Login ou senha inválidos", unauthorizedResult.Value);
        }

        [Fact]
        public async Task Create_DeveRetornarOk_QuandoUsuarioCriadoComSucesso()
        {
            // Arrange
            var criarUsuarioRequest = new CriarUsuarioRequestDTO
            {
                Nome = "Novo Usuario",
                Email = "novousuario@gmail.com",
                CPF = "111.222.333-44",
                TipoUsuario = TipoUsuario.Aluno,
                Senha = "Senha123"
            };

            _usuarioServiceMock.Setup(service => service.CriarUsuarioAsync(criarUsuarioRequest))
                               .ReturnsAsync(1);

            // Act
            var result = await _controller.Create(criarUsuarioRequest);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Listar_DeveRetornarListaDeUsuarios_QuandoSolicitado()
        {
            // Arrange
            var filtroRequest = new ListarUsuarioRequestDTO
            {
                nrPagina = 1,
                qtRegistros = 10
            };

            var usuarios = new PaginacaoResult<UsuarioDTO>
            {
                Itens = new List<UsuarioDTO>
                {
                    new UsuarioDTO { IdUsuario = 1, Nome = "Aluno1", Email = "aluno1@gmail.com", RA = "2025002" },
                    new UsuarioDTO { IdUsuario = 2, Nome = "Aluno2", Email = "aluno2@gmail.com", RA = "2025003" }
                },
                PaginaAtual = 1,
                TamanhoPagina = 10,
                TotalItens = 2,
                TotalPaginas = 1
            };

            _usuarioServiceMock.Setup(service => service.ListarUsuarios(filtroRequest))
                               .ReturnsAsync(usuarios);

            // Act
            var result = await _controller.Listar(filtroRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultValue = Assert.IsType<PaginacaoResult<UsuarioDTO>>(okResult.Value);
            Assert.Equal(2, resultValue.Itens.Count());
        }

        [Fact]
        public async Task Deletar_DeveRetornarOk_QuandoUsuarioDeletado()
        {
            // Arrange
            var usuarioId = 1;

            _usuarioServiceMock.Setup(service => service.DeletarUsuario(usuarioId))
                               .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Deletar(usuarioId);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Deletar_DeveLancarExcecao_QuandoUsuarioComTurmas()
        {
            // Arrange
            var usuarioId = 1;

            _usuarioServiceMock.Setup(service => service.DeletarUsuario(usuarioId))
                               .ThrowsAsync(new Exception("Não é possivel remover o usuário, será necessário remover seu vinculo com suas turmas primeiro."));

            // Act
            var result = await _controller.Deletar(usuarioId);

            // Assert
            var exceptionResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Não é possivel remover o usuário, será necessário remover seu vinculo com suas turmas primeiro.", exceptionResult.Value);
        }

        [Fact]
        public async Task Editar_DeveRetornarOk_QuandoUsuarioEditadoComSucesso()
        {
            // Arrange
            var editarUsuarioRequest = new EditarUsuarioRequestDTO
            {
                Id = 1,
                Nome = "Aluno Editado",
                Email = "alunoeditado@gmail.com",
                CPF = "123.456.789-99",
                Senha = "NovaSenha123"
            };

            _usuarioServiceMock.Setup(service => service.EditarUsuario(editarUsuarioRequest))
                               .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Editar(editarUsuarioRequest);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task ObterUsuario_DeveRetornarUsuario_QuandoIdValido()
        {
            // Arrange
            var usuarioId = 1;
            var usuarioDto = new UsuarioDTO
            {
                IdUsuario = 1,
                Nome = "Aluno1",
                Email = "aluno1@gmail.com",
                RA = "2025002",
                TipoUsuario = TipoUsuario.Aluno
            };

            _usuarioServiceMock.Setup(service => service.ObterUsuarioPorId(usuarioId))
                               .ReturnsAsync(usuarioDto);

            // Act
            var result = await _controller.ObterUsuario(usuarioId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var usuario = Assert.IsType<UsuarioDTO>(okResult.Value);
            Assert.Equal("Aluno1", usuario.Nome);
        }
    }
}