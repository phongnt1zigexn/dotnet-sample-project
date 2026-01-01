using Microsoft.EntityFrameworkCore;
using SampleApi.Entities;

namespace SampleApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(256);
        });

        // Seed data
        var now = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Email = "admin@example.com",
                // Password: Admin@123
                PasswordHash = "$2a$11$rBLRDFQOVt0hJdMbQK1.YOwBjP7dOJHR1rDvQKJ6JyfBqxv.h8W2e",
                FullName = "Admin User",
                CreatedAt = now,
                UpdatedAt = now
            },
            new User
            {
                Id = 2,
                Email = "user1@example.com",
                // Password: User1@123
                PasswordHash = "$2a$11$8HGKy.pFWS6FcK4VQPwqJOaQCJZLF1CzFQKHwE36WdGhTKv5HmKrW",
                FullName = "User One",
                CreatedAt = now,
                UpdatedAt = now
            },
            new User
            {
                Id = 3,
                Email = "user2@example.com",
                // Password: User2@123
                PasswordHash = "$2a$11$QQ7fLVCdVJVQ7wDXj.F9YOV/1kQGwWzjPtLRJpQpAnWKmE2/xWXD6",
                FullName = "User Two",
                CreatedAt = now,
                UpdatedAt = now
            },
            new User
            {
                Id = 4,
                Email = "john.doe@example.com",
                // Password: John@123
                PasswordHash = "$2a$11$xyz123abc456def789ghijklmnopqrstuvwxyz123456789",
                FullName = "John Doe",
                CreatedAt = now,
                UpdatedAt = now
            },
            new User
            {
                Id = 5,
                Email = "jane.smith@example.com",
                // Password: Jane@123
                PasswordHash = "$2a$11$abc123xyz456uvw789defghijklmnopqrstuvwxyz98765",
                FullName = "Jane Smith",
                CreatedAt = now,
                UpdatedAt = now
            }
        );
    }
}
