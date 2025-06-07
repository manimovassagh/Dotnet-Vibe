using Microsoft.EntityFrameworkCore;
using SqliteWebApi.Models;

namespace SqliteWebApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Apartment> Apartments { get; set; }
    public DbSet<ApartmentPhoto> ApartmentPhotos { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Apartment>()
            .HasMany(a => a.Photos)
            .WithOne(p => p.Apartment)
            .HasForeignKey(p => p.ApartmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
