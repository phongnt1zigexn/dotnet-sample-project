using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using SampleApi.Controllers;
using SampleApi.Data;
using SampleApi.DTOs;
using SampleApi.Tests.Fixtures;
using SampleApi.Tests.Helpers;

namespace SampleApi.Tests.Controllers;

public class UsersControllerTests : IClassFixture<TestDbContextFixture>
{
    private readonly TestDbContextFixture _fixture;

    public UsersControllerTests(TestDbContextFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetUsers_WithDefaultPagination_ReturnsFirstPage()
    {
        // Arrange
        var context = CreateTestDbContext();
        var controller = new UsersController(context);

        // Act
        var result = await controller.GetUsers();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeOfType<UserListResponse>();
        
        var response = okResult.Value as UserListResponse;
        response!.Users.Should().HaveCount(3); // Default pageSize is 10, but we only have 3 users
        response.TotalCount.Should().Be(3);
        response.Page.Should().Be(1);
        response.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task GetUsers_WithCustomPagination_ReturnsCorrectPage()
    {
        // Arrange
        var context = CreateTestDbContext();
        var controller = new UsersController(context);

        // Act
        var result = await controller.GetUsers(page: 1, pageSize: 2);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var response = okResult!.Value as UserListResponse;
        
        response!.Users.Should().HaveCount(2);
        response.Page.Should().Be(1);
        response.PageSize.Should().Be(2);
        response.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task GetUsers_WithPageSizeGreaterThanMax_ReturnsMaxPageSize()
    {
        // Arrange
        var context = CreateTestDbContext();
        var controller = new UsersController(context);

        // Act
        var result = await controller.GetUsers(page: 1, pageSize: 150);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var response = okResult!.Value as UserListResponse;
        
        response!.PageSize.Should().Be(100); // Max pageSize is 100
    }

    [Fact]
    public async Task GetUsers_WithInvalidPageNumber_ReturnsFirstPage()
    {
        // Arrange
        var context = CreateTestDbContext();
        var controller = new UsersController(context);

        // Act
        var result = await controller.GetUsers(page: 0, pageSize: 10);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var response = okResult!.Value as UserListResponse;
        
        response!.Page.Should().Be(1); // Should default to 1
    }

    [Fact]
    public async Task GetUsers_WithInvalidPageSize_ReturnsDefaultPageSize()
    {
        // Arrange
        var context = CreateTestDbContext();
        var controller = new UsersController(context);

        // Act
        var result = await controller.GetUsers(page: 1, pageSize: 0);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var response = okResult!.Value as UserListResponse;
        
        response!.PageSize.Should().Be(10); // Should default to 10
    }

    [Fact]
    public async Task GetUser_WithValidId_ReturnsUser()
    {
        // Arrange
        var context = CreateTestDbContext();
        var controller = new UsersController(context);

        // Act
        var result = await controller.GetUser(1);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeOfType<UserDto>();
        
        var userDto = okResult.Value as UserDto;
        userDto!.Id.Should().Be(1);
        userDto.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task GetUser_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var context = CreateTestDbContext();
        var controller = new UsersController(context);

        // Act
        var result = await controller.GetUser(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetUsers_ReturnsUsersOrderedById()
    {
        // Arrange
        var context = CreateTestDbContext();
        var controller = new UsersController(context);

        // Act
        var result = await controller.GetUsers();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var response = okResult!.Value as UserListResponse;
        
        response!.Users.Should().BeInAscendingOrder(u => u.Id);
    }

    private AppDbContext CreateTestDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        
        // Seed test data
        var users = new List<SampleApi.Entities.User>
        {
            new SampleApi.Entities.User
            {
                Id = 1,
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123!"),
                FullName = "Test User",
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new SampleApi.Entities.User
            {
                Id = 2,
                Email = "user2@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                FullName = "User Two",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new SampleApi.Entities.User
            {
                Id = 3,
                Email = "user3@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                FullName = "User Three",
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                UpdatedAt = DateTime.UtcNow.AddDays(-3)
            }
        };

        context.Users.AddRange(users);
        context.SaveChanges();

        return context;
    }
}

