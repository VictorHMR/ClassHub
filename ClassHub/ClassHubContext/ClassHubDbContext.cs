using ClassHub.ClassHubContext.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassHub.ClassHubContext
{
    public class ClassHubDbContext : DbContext
    {
        public ClassHubDbContext(DbContextOptions<ClassHubDbContext> options)
            : base(options) { }

        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Turma> Turmas => Set<Turma>();
        public DbSet<AlunoTurma> AlunoTurmas => Set<AlunoTurma>();
        public DbSet<Nota> Notas => Set<Nota>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Turma>()
                .HasOne(t => t.Professor)
                .WithMany(u => u.TurmasLecionadas)
                .HasForeignKey(t => t.IdProfessor)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AlunoTurma>()
                .HasOne(at => at.Aluno)
                .WithMany(u => u.Matriculas)
                .HasForeignKey(at => at.IdAluno)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AlunoTurma>()
                .HasOne(at => at.Turma)
                .WithMany(t => t.Matriculas)
                .HasForeignKey(at => at.IdTurma)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AlunoTurma>()
                .HasIndex(at => new { at.IdAluno, at.IdTurma })
                .IsUnique();

            modelBuilder.Entity<Nota>()
                .HasOne(an => an.AlunoTurma)
                .WithMany(at => at.NotasLancadas)
                .HasForeignKey(an => an.IdAlunoTurma)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
