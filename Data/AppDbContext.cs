using AssetManager.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Project> Projects { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Specify SQLite as the database provider and the file location
        optionsBuilder.UseSqlite("Data Source=appdata.db");

    }

    public AppDbContext()
    {
        Projects = Set<Project>();
    }
}
