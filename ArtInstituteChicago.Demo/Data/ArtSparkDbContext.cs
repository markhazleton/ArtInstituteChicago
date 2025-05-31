using Microsoft.EntityFrameworkCore;
using ArtInstituteChicago.Demo.Models;

namespace ArtInstituteChicago.Demo.Data;

public class ArtSparkDbContext : DbContext
{
    public ArtSparkDbContext(DbContextOptions<ArtSparkDbContext> options) : base(options)
    {
    }

    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<ArtworkReview> Reviews { get; set; }
    public DbSet<UserFavorite> Favorites { get; set; }
    public DbSet<UserCollection> Collections { get; set; }
    public DbSet<CollectionArtwork> CollectionArtworks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure ApplicationUser
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.UserName).IsUnique();
        });

        // Configure ArtworkReview
        modelBuilder.Entity<ArtworkReview>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.ArtworkId }).IsUnique();

            entity.HasOne(e => e.User)
                  .WithMany(u => u.Reviews)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure UserFavorite
        modelBuilder.Entity<UserFavorite>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.ArtworkId }).IsUnique();

            entity.HasOne(e => e.User)
                  .WithMany(u => u.Favorites)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure UserCollection
        modelBuilder.Entity<UserCollection>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.User)
                  .WithMany(u => u.Collections)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure CollectionArtwork
        modelBuilder.Entity<CollectionArtwork>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.CollectionId, e.ArtworkId }).IsUnique();

            entity.HasOne(e => e.Collection)
                  .WithMany(c => c.Artworks)
                  .HasForeignKey(e => e.CollectionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
