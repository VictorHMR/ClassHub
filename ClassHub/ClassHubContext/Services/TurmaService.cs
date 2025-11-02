using ClassHub.ClassHubContext.Models;
using ClassHub.Dtos.Turma;
using ClassHub.Enums;
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

        public virtual async Task<int> CriarTurmaAsync (CriarTurmaRequestDTO novaTurma)
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

            return Turma.Id;
        }

        public virtual async Task EditarTurmaAsync(EditarTurmaRequestDTO turma)
        {
            Turma? turmaDB = await _db.Turmas.FindAsync(turma.IdTurma);

            if(turmaDB is null)
                throw new Exception("Turma não encontrada");

            turmaDB.Nome = turma.Nome;
            turmaDB.IdProfessor = turma.IdProfessor;
            turmaDB.DtFim = turma.DtFim;

            await _db.SaveChangesAsync();
        }

        public virtual async Task DeletarTurmaAsync(int idTurma)
        {
            Turma? turma = await _db.Turmas.FindAsync(idTurma);
            if (turma is null)
                throw new Exception("Turma não encontrada");

            var possuiAlunos = _db.AlunoTurmas.Any(at => at.IdTurma == idTurma);

            if(possuiAlunos)
                throw new Exception("Existem alunos vinculados a turma, favor remove-los antes de efetuar a deleção");

            _db.Turmas.Remove(turma);
            await _db.SaveChangesAsync();
        }

        public virtual async Task<PaginacaoResult<TurmaDTO>> ListarTurmasAsync(ListarTurmaRequestDTO request)
        {
            if (request.nrPagina< 1) request.nrPagina = 1;
            if (request.qtRegistros < 1) request.qtRegistros = 10;
            var query = _db.Turmas
                .Include(t => t.Professor)
                .Include(u => u.Matriculas)
                .OrderBy(t => t.Nome)
                .AsQueryable();

            if (request.idUsuario != null)
                query = query.Where(u => u.Matriculas.Any(x => x.IdAluno == request.idUsuario) || u.IdProfessor == request.idUsuario);

            if (!string.IsNullOrEmpty(request.pesquisa))
                query = query.Where(u => u.Nome.StartsWith(request.pesquisa));

            if (request.ordenacao == Filtros.Ordenacao.Ascendente)
                query = query.OrderBy(t => t.Nome);
            else
                query = query.OrderByDescending(t => t.Nome);

            var total = await query.CountAsync();

            var turmas = await query
                .Skip((request.nrPagina - 1) * request.qtRegistros)
                .Take(request.qtRegistros)
                .Select(t => new TurmaDTO
                {
                    IdTurma = t.Id,
                    Nome = t.Nome,
                    DtInicio = t.DtInicio,
                    DtFim = t.DtFim,
                    QtdAlunos = t.Matriculas.Count,
                    NomeProfessor = t.Professor.Nome
                }).ToListAsync();

            return new PaginacaoResult<TurmaDTO>
            {
                Itens = turmas,
                PaginaAtual = request.nrPagina,
                TamanhoPagina = request.qtRegistros,
                TotalItens = total,
                TotalPaginas = (int)Math.Ceiling(total / (double)request.qtRegistros)
            };
        }

        public virtual async Task VincularAlunoTurmaAsync(VincularAlunoTurmaRequestDTO request)
        {
            var idAluno = _db.Usuarios
                .Where(u => u.RA == request.RAAluno)
                .Select(u => u.Id)
                .FirstOrDefault();

            var alunoTurma = await _db.AlunoTurmas
                .FirstOrDefaultAsync(at => at.IdAluno == idAluno && at.IdTurma == request.IdTurma);

            if (alunoTurma is null && !request.FlDesvincular)
            {
                alunoTurma = new AlunoTurma
                {
                    IdAluno = idAluno,
                    IdTurma = request.IdTurma
                };
                _db.AlunoTurmas.Add(alunoTurma);
            }
            else
            {
                if (request.FlDesvincular && alunoTurma is not null)
                {
                    _db.AlunoTurmas.Remove(alunoTurma!);
                }
            }
            await _db.SaveChangesAsync();
        }

        public virtual async Task<TurmaDTO?> ObterTurmaPorId(int idTurma)
        {
            var turmaDB = await _db.Turmas.Include(t=> t.Professor).Include(t=> t.Matriculas).FirstOrDefaultAsync(t => t.Id == idTurma);

            if (turmaDB is null) return null;

            return new TurmaDTO
            {
                IdTurma = turmaDB.Id,
                Nome = turmaDB.Nome,
                DtInicio = turmaDB.DtInicio,
                DtFim = turmaDB.DtFim,
                NomeProfessor = turmaDB.Professor.Nome,
                IdProfessor = turmaDB.IdProfessor,
                QtdAlunos = turmaDB.Matriculas.Count()
                
            };
        }


    }
}
