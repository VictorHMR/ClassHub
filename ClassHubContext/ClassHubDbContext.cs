using ClassHub.ClassHubContext.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassHub.ClassHubContext
{
    public class ClassHubDbContext : DbContext
    {
        public ClassHubDbContext(DbContextOptions<ClassHubDbContext> options)
            : base(options) { }

        public DbSet<Usuario> Usuarios => Set<Usuario>();

    }
}
