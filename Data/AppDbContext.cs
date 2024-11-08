using AssetManager.Models;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Imaging;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

public class AppDbContext : DbContext
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<AssetMetadata> MetadataFiles { get; set; }
    public DbSet<Asset> Assets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Specify SQLite as the database provider and the file location
        optionsBuilder.UseSqlite("Data Source=appdata.db");

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Asset>()
             .HasOne<Project>()          
             .WithMany()                 
             .HasForeignKey(a => a.ProjectId)
             .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<Asset>()
            .HasOne(a => a.Metadata)            
            .WithOne(m => m.Asset)             
            .HasForeignKey<AssetMetadata>(m => m.AssetId) // Set AssetId in AssetMetadata as the foreign key
            .OnDelete(DeleteBehavior.Cascade);
    }
    public AppDbContext()
    {
        Projects = Set<Project>();
        MetadataFiles = Set<AssetMetadata>();
        Assets = Set<Asset>();

        this.Database.EnsureCreated();
    }
}
