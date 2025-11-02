using ClassHub.ClassHubContext;
using ClassHub.ClassHubContext.Models;
using ClassHub.ClassHubContext.Services;
using ClassHub.Dtos.Nota;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ClassHub.Tests
{
    public class NotaServiceTests
    {
        private ClassHubDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ClassHubDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ClassHubDbContext(options);
        }

        [Fact]
        public async Task InserirNotaAsync_DeveInserirNota()
        {
            // Arrange
            var db = GetDbContext();
            var alunoTurma = new AlunoTurma { Id = 1 };
            db.AlunoTurmas.Add(alunoTurma);
            await db.SaveChangesAsync();

            var service = new NotaService(db);
            var dto = new LancarNotaRequestDTO
            {
                IdAlunoTurma = alunoTurma.Id,
                Nota = 8.5,
                Descricao = "Prova Final"
            };

            // Act
            await service.InserirNotaAsync(dto);
            var notaInserida = await db.Notas.FirstOrDefaultAsync(n => n.IdAlunoTurma == alunoTurma.Id);

            // Assert
            Assert.NotNull(notaInserida);
            Assert.Equal(dto.Nota, notaInserida.Valor);
            Assert.Equal(dto.Descricao, notaInserida.Descricao);
        }

        [Fact]
        public async Task InserirNotaAsync_DeveLancarExcecao_AlunoNaoMatriculado()
        {
            // Arrange
            var db = GetDbContext();
            var service = new NotaService(db);
            var dto = new LancarNotaRequestDTO
            {
                IdAlunoTurma = 999,
                Nota = 9.0,
                Descricao = "Prova Final"
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.InserirNotaAsync(dto));
        }

        [Fact]
        public async Task EditarNotaAsync_DeveEditarNota()
        {
            // Arrange
            var db = GetDbContext();
            var alunoTurma = new AlunoTurma { Id = 1 };
            db.AlunoTurmas.Add(alunoTurma);
            var nota = new Nota
            {
                IdAlunoTurma = alunoTurma.Id,
                Valor = 7.5,
                Descricao = "Prova Parcial"
            };
            db.Notas.Add(nota);
            await db.SaveChangesAsync();

            var service = new NotaService(db);
            var dto = new EditarNotaRequestDTO
            {
                IdNota = nota.Id,
                Nota = 9.0,
                Descricao = "Prova Final"
            };

            // Act
            await service.EditarNotaAsync(dto);
            var notaEditada = await db.Notas.FindAsync(nota.Id);

            // Assert
            Assert.Equal(dto.Nota, notaEditada.Valor);
            Assert.Equal(dto.Descricao, notaEditada.Descricao);
        }

        [Fact]
        public async Task EditarNotaAsync_DeveLancarExcecao_NotaNaoEncontrada()
        {
            // Arrange
            var db = GetDbContext();
            var service = new NotaService(db);
            var dto = new EditarNotaRequestDTO
            {
                IdNota = 999,
                Nota = 9.0,
                Descricao = "Prova Final"
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.EditarNotaAsync(dto));
        }

        [Fact]
        public async Task DeletarNotaAsync_DeveDeletarNota()
        {
            // Arrange
            var db = GetDbContext();
            var alunoTurma = new AlunoTurma { Id = 1 };
            db.AlunoTurmas.Add(alunoTurma);
            var nota = new Nota
            {
                IdAlunoTurma = alunoTurma.Id,
                Valor = 8.0,
                Descricao = "Prova Final"
            };
            db.Notas.Add(nota);
            await db.SaveChangesAsync();

            var service = new NotaService(db);

            // Act
            await service.DeletarNotaAsync(nota.Id);
            var notaDeletada = await db.Notas.FindAsync(nota.Id);

            // Assert
            Assert.Null(notaDeletada);
        }

        [Fact]
        public async Task DeletarNotaAsync_DeveLancarExcecao_NotaNaoEncontrada()
        {
            // Arrange
            var db = GetDbContext();
            var service = new NotaService(db);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.DeletarNotaAsync(999));
        }

        [Fact]
        public async Task ListarNotasAluno_DeveRetornarNotas()
        {
            // Arrange
            var db = GetDbContext();
            var alunoTurma = new AlunoTurma { Id = 1 };
            db.AlunoTurmas.Add(alunoTurma);
            var nota1 = new Nota
            {
                IdAlunoTurma = alunoTurma.Id,
                Valor = 7.0,
                Descricao = "Prova 1",
                DtLancamento = DateTime.Now
            };
            var nota2 = new Nota
            {
                IdAlunoTurma = alunoTurma.Id,
                Valor = 8.5,
                Descricao = "Prova 2",
                DtLancamento = DateTime.Now
            };
            db.Notas.Add(nota1);
            db.Notas.Add(nota2);
            await db.SaveChangesAsync();

            var service = new NotaService(db);

            // Act
            var notas = await service.ListarNotasAluno(alunoTurma.Id);

            // Assert
            Assert.Equal(2, notas.Count);
            Assert.Contains(notas, n => n.Nota == 7.0);
            Assert.Contains(notas, n => n.Nota == 8.5);
        }
    }
}