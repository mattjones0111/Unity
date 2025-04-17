using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Api.Host.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AudioCatalogDbContext>
{
    public AudioCatalogDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<AudioCatalogDbContext> optionsBuilder = new();

        optionsBuilder.UseNpgsql("Server=localhost;" +
                                 "User Id=admin;" +
                                 "Password=changeit;" +
                                 "port=5432;" +
                                 "Database=audio;" +
                                 "Pooling=false;");

        return new AudioCatalogDbContext(optionsBuilder.Options);
    }
}