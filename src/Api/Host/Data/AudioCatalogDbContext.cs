using Microsoft.EntityFrameworkCore;

namespace Api.Host.Data;

public class AudioCatalogDbContext : DbContext
{
    public AudioCatalogDbContext(DbContextOptions<AudioCatalogDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("unity_audio");

        modelBuilder.Entity<Category>()
            .HasKey(x => x.Path);
        
        modelBuilder.ApplySnakeCasing();
    }
}
