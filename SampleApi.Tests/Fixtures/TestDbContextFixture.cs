using Microsoft.EntityFrameworkCore;
using SampleApi.Data;
using SampleApi.Entities;

namespace SampleApi.Tests.Fixtures;

public class TestDbContextFixture : IDisposable
{
    public AppDbContext Context { get; private set; }

    public TestDbContextFixture()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new AppDbContext(options);
        SeedTestData();
    }

    private void SeedTestData()
    {
        // Add test users
        var users = new List<User>
        {
            new User
            {
                Id = 1,
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123!"),
                FullName = "Test User",
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new User
            {
                Id = 2,
                Email = "user2@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                FullName = "User Two",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new User
            {
                Id = 3,
                Email = "user3@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                FullName = "User Three",
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                UpdatedAt = DateTime.UtcNow.AddDays(-3)
            }
        };

        Context.Users.AddRange(users);
        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}

