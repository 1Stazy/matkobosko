using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace KinoApp.Infrastructure.Data
{
    // Używane przez narzędzia EF Core (dotnet ef migrations add ...)
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            // Domyślny connection string dla narzędzi — możesz to zmienić
            optionsBuilder.UseSqlite("Data Source=kino.db");
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
