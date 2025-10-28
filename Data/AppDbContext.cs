using Microsoft.EntityFrameworkCore;
using TaskManager.API.Models;

namespace TaskManager.API.Data
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // Configure relationship: TaskItem belongs to User
      modelBuilder.Entity<TaskItem>()
          .HasOne(t => t.User)
          .WithMany()
          .HasForeignKey(t => t.UserId)
          .OnDelete(DeleteBehavior.Cascade);

      // Add unique index on Email
      modelBuilder.Entity<User>()
          .HasIndex(u => u.Email)
          .IsUnique();

      // Add unique index on Username
      modelBuilder.Entity<User>()
          .HasIndex(u => u.Username)
          .IsUnique();
    }
  }
}