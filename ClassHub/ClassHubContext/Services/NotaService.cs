using ClassHub.ClassHubContext;
using ClassHub.ClassHubContext.Models;
using ClassHub.Dtos.Nota;
using Microsoft.EntityFrameworkCore;

public class NotaService
{
    private readonly ClassHubDbContext _db;
    public NotaService(ClassHubDbContext db)
    {
        _db = db;
    }

    public virtual async Task InserirNotaAsync(LancarNotaRequestDTO nota)
    {
        bool alunoTurmaExists = await _db.AlunoTurmas
            .AnyAsync(at => at.Id == nota.IdAlunoTurma);

        if (!alunoTurmaExists)
            throw new Exception("Aluno não está matriculado na turma especificada");

        var notaDB = new Nota
        {
            IdAlunoTurma = nota.IdAlunoTurma,
            Valor = nota.Nota,
            Descricao = nota.Descricao,
            DtLancamento = DateTime.Now
        };
        _db.Notas.Add(notaDB);
        await _db.SaveChangesAsync();
    }

    public virtual async Task EditarNotaAsync(EditarNotaRequestDTO novaNota)
    {
        Nota? notaDB = await _db.Notas.FindAsync(novaNota.IdNota);

        if (notaDB is null)
            throw new Exception("Lançamento de nota não encontrada");

        notaDB.Valor = novaNota.Nota;
        notaDB.Descricao = novaNota.Descricao;

        await _db.SaveChangesAsync();
    }

    public virtual async Task DeletarNotaAsync(int idNota)
    {
        Nota? notaDB = await _db.Notas.FindAsync(idNota);

        if (notaDB is null)
            throw new Exception("Lançamento de nota não encontrada");

        _db.Notas.Remove(notaDB);
        await _db.SaveChangesAsync();
    }

    public virtual async Task<List<ListarNotasAlunoResponseDTO>> ListarNotasAluno(int idAlunoTurma)
    {
        List<ListarNotasAlunoResponseDTO> notas = await _db.Notas
            .Where(n => n.IdAlunoTurma == idAlunoTurma)
            .Select(n => new ListarNotasAlunoResponseDTO
            {
                IdNota = n.Id,
                Nota = n.Valor,
                Descricao = n.Descricao,
                DtLancamento = n.DtLancamento
            }).ToListAsync();

        return notas;
    }
}