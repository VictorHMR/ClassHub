using ClassHub.ClassHubContext;
using ClassHub.ClassHubContext.Models;
using ClassHub.ClassHubContext.Services;
using ClassHub.Dtos.Usuario;
using ClassHub.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace ClassHub.Tests
{
    public class UsuarioServiceTests
    {
        private ClassHubDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ClassHubDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ClassHubDbContext(options);
        }

        [Fact]
        public async Task CriarUsuarioAsync_DeveCriarUsuario()
        {
            // Arrange
            var db = GetDbContext();
            var service = new UsuarioService(db, null);
            var dto = new CriarUsuarioRequestDTO
            {
                Nome = "Aluno1",
                Email = "aluno1@gmail.com",
                CPF = "123.456.789-00",
                TipoUsuario = TipoUsuario.Aluno,
                Senha = "Senha123"
            };

            // Act
            var id = await service.CriarUsuarioAsync(dto);
            var usuarioCriado = await db.Usuarios.FindAsync(id);

            // Assert
            Assert.NotNull(usuarioCriado);
            Assert.Equal(dto.Nome, usuarioCriado.Nome);
            Assert.Equal(dto.Email, usuarioCriado.Email);
            Assert.Equal(dto.CPF, usuarioCriado.CPF);
            Assert.NotNull(usuarioCriado.Senha);
        }

        [Fact]
        public async Task ObterUsuarioAsync_DeveRetornarUsuarioComToken()
        {
            // Arrange
            var db = GetDbContext();

            var usuario = new Usuario
            {
                Nome = "Aluno1",
                Email = "aluno1@gmail.com",
                CPF = "123.456.789-00",
                Senha = new PasswordHasher<Usuario>().HashPassword(null, "Senha123"),
                RA = "2025002"
            };

            db.Usuarios.Add(usuario);
            await db.SaveChangesAsync();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
            { "Jwt:Key", "2eb50905b3aaa3bfb6ad77a7697c0d5b" },
            { "Jwt:ExpireHours", "1" }
                })
                .Build();

            var service = new UsuarioService(db, configuration);
            var loginRequest = new LoginRequestDTO
            {
                Login = "aluno1@gmail.com",
                Senha = "Senha123"
            };

            // Act
            var usuarioRetornado = await service.ObterUsuarioAsync(loginRequest);

            // Assert
            Assert.NotNull(usuarioRetornado);
            Assert.Equal("Aluno1", usuarioRetornado?.Nome);
            Assert.Equal("aluno1@gmail.com", usuarioRetornado?.Email);
            Assert.NotNull(usuarioRetornado?.Token);
        }

        [Fact]
        public async Task ObterUsuarioAsync_DeveRetornarNullParaLoginInvalido()
        {
            // Arrange
            var db = GetDbContext();

            var usuario = new Usuario
            {
                Nome = "Aluno1",
                Email = "aluno1@gmail.com",
                CPF = "123.456.789-00",
                Senha = new PasswordHasher<Usuario>().HashPassword(null, "Senha123"),
                RA = "2025002"
            };

            db.Usuarios.Add(usuario);
            await db.SaveChangesAsync();

            var service = new UsuarioService(db, null);
            var loginRequest = new LoginRequestDTO
            {
                Login = "inexistente@gmail.com",
                Senha = "SenhaErrada"
            };

            // Act
            var usuarioRetornado = await service.ObterUsuarioAsync(loginRequest);

            // Assert
            Assert.Null(usuarioRetornado);
        }


        [Fact]
        public async Task DeletarUsuario_DeveDeletarUsuario()
        {
            // Arrange
            var db = GetDbContext();
            var usuario = new Usuario
            {
                Nome = "Aluno2",
                Email = "aluno2@gmail.com",
                CPF = "123.456.789-00",
                Senha = new PasswordHasher<Usuario>().HashPassword(null, "Senha123"),
                RA = "2025003"
            };
            db.Usuarios.Add(usuario);
            await db.SaveChangesAsync();

            var service = new UsuarioService(db, null);

            // Act
            await service.DeletarUsuario(usuario.Id);
            var usuarioDeletado = await db.Usuarios.FindAsync(usuario.Id);

            // Assert
            Assert.Null(usuarioDeletado);
        }

        [Fact]
        public async Task DeletarUsuario_DeveLancarExcecao_UsuarioComTurmas()
        {
            // Arrange
            var db = GetDbContext();

            var usuario = new Usuario
            {
                Nome = "Aluno3",
                Email = "aluno3@gmail.com",
                CPF = "123.456.789-00",
                Senha = new PasswordHasher<Usuario>().HashPassword(null, "Senha123"),
                RA = "2025004"
            };

            db.Usuarios.Add(usuario);
            await db.SaveChangesAsync();

            var turma = new Turma { Nome = "Turma Teste" };
            db.Turmas.Add(turma);
            await db.SaveChangesAsync();

            db.AlunoTurmas.Add(new AlunoTurma { IdAluno = usuario.Id, IdTurma = turma.Id });
            await db.SaveChangesAsync();

            var service = new UsuarioService(db, null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.DeletarUsuario(usuario.Id));
        }

        [Fact]
        public async Task ListarUsuarios_DeveRetornarListaDeUsuarios()
        {
            // Arrange
            var db = GetDbContext();

            var usuario1 = new Usuario
            {
                Nome = "Aluno1",
                Email = "aluno1@gmail.com",
                CPF = "123.456.789-00",
                Senha = new PasswordHasher<Usuario>().HashPassword(null, "Senha123"),
                RA = "2025002"
            };

            var usuario2 = new Usuario
            {
                Nome = "Aluno2",
                Email = "aluno2@gmail.com",
                CPF = "987.654.321-00",
                Senha = new PasswordHasher<Usuario>().HashPassword(null, "Senha123"),
                RA = "2025003"
            };

            db.Usuarios.Add(usuario1);
            db.Usuarios.Add(usuario2);
            await db.SaveChangesAsync();

            var service = new UsuarioService(db, null);
            var request = new ListarUsuarioRequestDTO
            {
                nrPagina = 1,
                qtRegistros = 10
            };

            // Act
            var resultado = await service.ListarUsuarios(request);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Itens.Count());
            Assert.Contains(resultado.Itens, u => u.Nome == "Aluno1");
            Assert.Contains(resultado.Itens, u => u.Nome == "Aluno2");
        }


        [Fact]
        public async Task EditarUsuario_DeveEditarUsuario()
        {
            // Arrange
            var db = GetDbContext();

            var usuario = new Usuario
            {
                Nome = "Aluno6",
                Email = "aluno6@gmail.com",
                CPF = "123.456.789-11",
                Senha = new PasswordHasher<Usuario>().HashPassword(null, "Senha123"),
                RA = "2025008"
            };

            db.Usuarios.Add(usuario);
            await db.SaveChangesAsync();

            var editarDto = new EditarUsuarioRequestDTO
            {
                Id = usuario.Id,
                Nome = "Aluno6 Editado",
                Email = "aluno6editado@gmail.com",
                CPF = "123.456.789-12",
                Senha = "NovaSenha123"
            };

            var service = new UsuarioService(db, null);

            // Act
            await service.EditarUsuario(editarDto);
            var usuarioEditado = await db.Usuarios.FindAsync(usuario.Id);

            // Assert
            Assert.Equal(editarDto.Nome, usuarioEditado?.Nome);
            Assert.Equal(editarDto.Email, usuarioEditado?.Email);
            Assert.Equal(editarDto.CPF, usuarioEditado?.CPF);

            var passwordHasher = new PasswordHasher<Usuario>();
            var senhaVerificada = passwordHasher.VerifyHashedPassword(usuarioEditado, usuarioEditado?.Senha, editarDto.Senha);
            Assert.Equal(PasswordVerificationResult.Success, senhaVerificada);
        }

        [Fact]
        public async Task EditarUsuario_DeveLancarExcecao_UsuarioNaoEncontrado()
        {
            // Arrange
            var db = GetDbContext();
            var service = new UsuarioService(db, null);
            var editarDto = new EditarUsuarioRequestDTO
            {
                Id = 999,
                Nome = "Usuario Inexistente",
                Email = "naoexiste@teste.com"
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.EditarUsuario(editarDto));
        }
    }
}