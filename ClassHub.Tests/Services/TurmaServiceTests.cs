using ClassHub.ClassHubContext;
using ClassHub.ClassHubContext.Models;
using ClassHub.ClassHubContext.Services;
using ClassHub.Dtos.Turma;
using ClassHub.Enums;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ClassHub.Tests
{
    public class TurmaServiceTests
    {
        private ClassHubDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ClassHubDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ClassHubDbContext(options);
        }

        [Fact]
        public async Task CriarTurmaAsync_DeveCriarTurma()
        {
            // Arrange
            var db = GetDbContext();
            var service = new TurmaService(db, null);

            var dto = new CriarTurmaRequestDTO
            {
                Nome = "Turma Teste",
                IdProfessor = 1
            };

            // Act
            var id = await service.CriarTurmaAsync(dto);
            var turmaCriada = await db.Turmas.FindAsync(id);

            // Assert
            Assert.NotNull(turmaCriada);
            Assert.Equal(dto.Nome, turmaCriada.Nome);
            Assert.Equal(dto.IdProfessor, turmaCriada.IdProfessor);
        }

        [Fact]
        public async Task EditarTurmaAsync_DeveEditarTurma()
        {
            // Arrange
            var db = GetDbContext();
            var turma = new Turma { Nome = "Old", IdProfessor = 1 };
            db.Turmas.Add(turma);
            await db.SaveChangesAsync();

            var service = new TurmaService(db, null);
            var dto = new EditarTurmaRequestDTO
            {
                IdTurma = turma.Id,
                Nome = "Novo Nome",
                IdProfessor = 2,
                DtFim = DateTime.Today
            };

            // Act
            await service.EditarTurmaAsync(dto);
            var turmaEditada = await db.Turmas.FindAsync(turma.Id);

            // Assert
            Assert.Equal("Novo Nome", turmaEditada.Nome);
            Assert.Equal(2, turmaEditada.IdProfessor);
            Assert.Equal(DateTime.Today, turmaEditada.DtFim);
        }

        [Fact]
        public async Task DeletarTurmaAsync_DeveRemoverTurma()
        {
            // Arrange
            var db = GetDbContext();
            var turma = new Turma { Nome = "Turma Remover" };
            db.Turmas.Add(turma);
            await db.SaveChangesAsync();

            var service = new TurmaService(db, null);

            // Act
            await service.DeletarTurmaAsync(turma.Id);
            var turmaDeletada = await db.Turmas.FindAsync(turma.Id);

            // Assert
            Assert.Null(turmaDeletada);
        }

        [Fact]
        public async Task VincularAlunoTurmaAsync_DeveVincularAluno()
        {
            // Arrange
            var db = GetDbContext();
            var aluno = new Usuario { Nome = "Teste", CPF = "123.456.789-10", RA = "123", Senha = "123", TipoUsuario = TipoUsuario.Aluno, Email = "teste@teste.com" };
            var turma = new Turma { Id = 1, Nome = "Turma 1" };
            db.Usuarios.Add(aluno);
            db.Turmas.Add(turma);
            await db.SaveChangesAsync();

            var service = new TurmaService(db, null);
            var request = new VincularAlunoTurmaRequestDTO
            {
                IdTurma = turma.Id,
                RAAluno = "123",
                FlDesvincular = false
            };

            // Act
            await service.VincularAlunoTurmaAsync(request);
            var vinculo = await db.AlunoTurmas.FirstOrDefaultAsync(at => at.IdAluno == aluno.Id && at.IdTurma == turma.Id);

            // Assert
            Assert.NotNull(vinculo);
        }

        [Fact]
        public async Task ObterTurmaPorId_DeveRetornarTurma()
        {
            // Arrange
            var db = GetDbContext();
            var professor = new Usuario { Nome = "Teste", CPF = "123.456.789-10", RA = "123", Senha = "123", TipoUsuario = TipoUsuario.Professor, Email = "teste@teste.com" };
            var turma = new Turma { Id = 1, Nome = "Turma Teste", IdProfessor = professor.Id, Professor = professor };
            db.Usuarios.Add(professor);
            db.Turmas.Add(turma);
            await db.SaveChangesAsync();

            var service = new TurmaService(db, null);

            // Act
            var dto = await service.ObterTurmaPorId(1);

            // Assert
            Assert.NotNull(dto);
            Assert.Equal("Turma Teste", dto.Nome);
            Assert.Equal(professor.Nome, dto.NomeProfessor);
        }

        [Fact]
        public async Task ListarTurmasAsync_DeveRetornarPaginacao()
        {
            // Arrange
            var db = GetDbContext();
            db.Usuarios.Add(new Usuario { Nome = "Teste", CPF = "123.456.789-10", RA = "123", Senha = "123", TipoUsuario = TipoUsuario.Professor, Email = "teste@teste.com" });
            db.Turmas.AddRange(
                new Turma { Nome = "A", IdProfessor = 1 },
                new Turma { Nome = "B", IdProfessor = 1 }
            );
            await db.SaveChangesAsync();


            var service = new TurmaService(db, null);
            var request = new ListarTurmaRequestDTO { nrPagina = 1, qtRegistros = 1 };

            // Act
            var result = await service.ListarTurmasAsync(request);

            // Assert
            Assert.Equal(1, result.Itens.Count());
            Assert.Equal(2, result.TotalItens);
            Assert.Equal(2, result.TotalPaginas);
        }
    }
}
