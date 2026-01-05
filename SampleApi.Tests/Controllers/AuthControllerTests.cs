using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using SampleApi.Controllers;
using SampleApi.Data;
using SampleApi.DTOs;
using SampleApi.Services;
using SampleApi.Tests.Fixtures;
using SampleApi.Tests.Helpers;

namespace SampleApi.Tests.Controllers;

public class AuthControllerTests : IClassFixture<TestDbContextFixture>
{
    private readonly TestDbContextFixture _fixture;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly IConfiguration _configuration;

    public AuthControllerTests(TestDbContextFixture fixture)
    {
        _fixture = fixture;
        _tokenServiceMock = new Mock<ITokenService>();
        _configuration = TestHelpers.CreateTestConfiguration();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var context = CreateTestDbContext();
        _tokenServiceMock.Setup(x => x.GenerateToken(It.IsAny<SampleApi.Entities.User>()))
            .Returns("test-jwt-token");
        
        var controller = new AuthController(context, _tokenServiceMock.Object, _configuration);
        var request = TestHelpers.CreateLoginRequest("test@example.com", "Test123!");

        // Act
        var result = await controller.Login(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeOfType<LoginResponse>();
        
        var response = okResult.Value as LoginResponse;
        response!.Token.Should().Be("test-jwt-token");
        response.User.Should().NotBeNull();
        response.User.Email.Should().Be("test@example.com");
        response.ExpiresIn.Should().Be(3600); // 60 minutes * 60 seconds
    }

    [Fact]
    public async Task Login_WithInvalidEmail_ReturnsUnauthorized()
    {
        // Arrange
        var context = CreateTestDbContext();
        var controller = new AuthController(context, _tokenServiceMock.Object, _configuration);
        var request = TestHelpers.CreateLoginRequest("nonexistent@example.com", "Test123!");

        // Act
        var result = await controller.Login(request);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var context = CreateTestDbContext();
        var controller = new AuthController(context, _tokenServiceMock.Object, _configuration);
        var request = TestHelpers.CreateLoginRequest("test@example.com", "WrongPassword");

        // Act
        var result = await controller.Login(request);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task SignUp_WithValidData_ReturnsCreatedWithUser()
    {
        // Arrange
        var context = CreateTestDbContext();
        var controller = new AuthController(context, _tokenServiceMock.Object, _configuration);
        var request = TestHelpers.CreateSignUpRequest("newuser@example.com", "NewPass123!", "New User");

        // Act
        var result = await controller.SignUp(request);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult!.Value.Should().BeOfType<UserDto>();
        
        var userDto = createdResult.Value as UserDto;
        userDto!.Email.Should().Be("newuser@example.com");
        userDto.FullName.Should().Be("New User");
        userDto.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task SignUp_WithDuplicateEmail_ReturnsConflict()
    {
        // Arrange
        var context = CreateTestDbContext();
        var controller = new AuthController(context, _tokenServiceMock.Object, _configuration);
        var request = TestHelpers.CreateSignUpRequest("test@example.com", "NewPass123!", "New User");

        // Act
        var result = await controller.SignUp(request);

        // Assert
        result.Result.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task SignUp_WithValidData_CreatesUserInDatabase()
    {
        // Arrange
        var context = CreateTestDbContext();
        var controller = new AuthController(context, _tokenServiceMock.Object, _configuration);
        var request = TestHelpers.CreateSignUpRequest("newuser2@example.com", "NewPass123!", "New User 2");

        // Act
        await controller.SignUp(request);

        // Assert
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == "newuser2@example.com");
        user.Should().NotBeNull();
        user!.FullName.Should().Be("New User 2");
        BCrypt.Net.BCrypt.Verify("NewPass123!", user.PasswordHash).Should().BeTrue();
    }

    [Fact]
    public async Task SignUp_WithValidData_HashesPassword()
    {
        // Arrange
        var context = CreateTestDbContext();
        var controller = new AuthController(context, _tokenServiceMock.Object, _configuration);
        var request = TestHelpers.CreateSignUpRequest("newuser3@example.com", "NewPass123!", "New User 3");

        // Act
        await controller.SignUp(request);

        // Assert
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == "newuser3@example.com");
        user.Should().NotBeNull();
        user!.PasswordHash.Should().NotBe("NewPass123!");
        BCrypt.Net.BCrypt.Verify("NewPass123!", user.PasswordHash).Should().BeTrue();
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
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        context.Users.AddRange(users);
        context.SaveChanges();

        return context;
    }
}

