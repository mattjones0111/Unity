using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Api.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CatalogDbContext>
{
    public CatalogDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<CatalogDbContext> optionsBuilder = new();

        optionsBuilder.UseNpgsql("Server=localhost;" +
                                 "User Id=admin;" +
                                 "Password=changeit;" +
                                 "port=5432;" +
                                 "Database=catalog;" +
                                 "Pooling=false;");

        return new CatalogDbContext(optionsBuilder.Options);
    }
}