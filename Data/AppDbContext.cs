﻿using AssetManager.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<AssetMetadata> MetadataFiles { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<AssetTag> AssetTags { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
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
          .HasForeignKey<AssetMetadata>(m => m.AssetId)
          .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AssetTag>()
            .HasKey(at => new { at.AssetId, at.TagId });

        modelBuilder.Entity<AssetTag>()
            .HasOne(at => at.Asset)
            .WithMany(a => a.AssetTags)
            .HasForeignKey(at => at.AssetId);

        modelBuilder.Entity<AssetTag>()
            .HasOne(at => at.Tag)
            .WithMany(t => t.AssetTags)
            .HasForeignKey(at => at.TagId);
    }
    public AppDbContext()
    {
        Projects = Set<Project>();
        MetadataFiles = Set<AssetMetadata>();
        Assets = Set<Asset>();
        Tags = Set<Tag>();
        AssetTags = Set<AssetTag>();


        this.Database.EnsureCreated();
    }
}
