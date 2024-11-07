using AssetManager.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<AssetMetadata> MetadataFiles{ get; set; }  

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Specify SQLite as the database provider and the file location
        optionsBuilder.UseSqlite("Data Source=appdata.db");

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AssetMetadata>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();
    }
    public AppDbContext()
    {
        Projects = Set<Project>();
        MetadataFiles = Set<AssetMetadata>();
    }
}
