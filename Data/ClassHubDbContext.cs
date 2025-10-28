using Microsoft.EntityFrameworkCore;

namespace ClassHubContext.Data
{
    public class ClassHubDbContext : DbContext
    {
        public ClassHubDbContext(DbContextOptions<ClassHubDbContext> options)
            : base(options) { }


    }
}
