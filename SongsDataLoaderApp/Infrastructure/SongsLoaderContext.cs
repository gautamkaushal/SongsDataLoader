using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SongsDataLoaderApp.Models;

namespace SongsDataLoaderApp.Infrastructure;

public class SongsLoaderContext:DbContext
{
    private readonly IConfiguration configuration;

    public SongsLoaderContext(
        DbContextOptions<SongsLoaderContext> options,
        IConfiguration configuration)
        :base(options)
    {
        this.configuration = configuration;
    }

    public DbSet<Genre> Genres { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Album> Albums { get; set; }
    public DbSet<Song> Songs{get;set;}
    public DbSet<User> Users { get; set; }
    public DbSet<Rating> Ratings{get;set;}
    public DbSet<UserPreference> UserPreferences { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(
            configuration.GetConnectionString("SongsData"));
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
