using ClassHub.ClassHubContext.Models;
using ClassHub.Dtos.Turma;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ClassHub.ClassHubContext.Services
{
    public class TurmaService
    {
        private readonly ClassHubDbContext _db;


        public TurmaService(ClassHubDbContext db, IConfiguration configuration)
        {
            _db = db;
        }

        public async Task<Turma> CriarTurmaAsync (CriarTurmaRequestDTO novaTurma)
        {
            var Turma = new Turma
            {
                Nome = novaTurma.Nome,
                DtInicio = DateTime.Now,
                DtFim = null,
                IdProfessor = novaTurma.IdProfessor
            };

            _db.Turmas.Add(Turma);
            await _db.SaveChangesAsync();

            return Turma;
        }
        //Alterar retorno para que PaginacaoResult tenha uma DTO
        public async Task<PaginacaoResult<ListarTurmaResponseDTO>> ListarTurmasAsync(int pagina = 1, int tamanhoPagina = 10)
        {
            if (pagina < 1) pagina = 1;
            if (tamanhoPagina < 1) tamanhoPagina = 10;
            var query = _db.Turmas
                .Include(t => t.Professor)
                .OrderBy(t => t.Nome)
                .AsQueryable();

            var total = await query.CountAsync();

            var turmas = await query
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .Select(t => new ListarTurmaResponseDTO
                {
                    IdTurma = t.Id,
                    Nome = t.Nome,
                    DtInicio = t.DtInicio,
                    DtFim = t.DtFim,
                    QtdAlunos = t.Matriculas.Count,
                    NomeProfessor = t.Professor.Nome
                })
                .ToListAsync();

            return new PaginacaoResult<ListarTurmaResponseDTO>
            {
                Itens = turmas,
                PaginaAtual = pagina,
                TamanhoPagina = tamanhoPagina,
                TotalItens = total,
                TotalPaginas = (int)Math.Ceiling(total / (double)tamanhoPagina)
            };
        }

        public async Task VincularAlunoTurmaAsync(VincularAlunoTurmaRequestDTO request)
        {
            var idAluno = _db.Usuarios
                .Where(u => u.RA == request.RAAluno)
                .Select(u => u.Id)
                .FirstOrDefault();

            var alunoTurma = new AlunoTurma
            {
                IdAluno = idAluno,
                IdTurma = request.IdTurma
            };
            _db.AlunoTurmas.Add(alunoTurma);
            await _db.SaveChangesAsync();
        }

    }
}
