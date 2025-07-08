namespace BlogAPI.Data;

using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
  public DbSet<User> Users { get; set; }
  public DbSet<Post> Posts { get; set; }
  public DbSet<Category> Categories { get; set; }
  public DbSet<Like> Likes { get; set; }
  public DbSet<Comment> Comments { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.Entity<Like>()
      .HasKey(l => new { l.UserId, l.PostId });

    modelBuilder.Entity<Like>()
      .HasOne(l => l.User)
      .WithMany(u => u.LikedPosts)
      .HasForeignKey(l => l.UserId)
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Like>()
      .HasOne(l => l.Post)
      .WithMany(p => p.LikedByUser)
      .HasForeignKey(l => l.PostId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}