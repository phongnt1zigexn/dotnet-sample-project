using Microsoft.Extensions.Configuration;
using SampleApi.DTOs;
using SampleApi.Entities;

namespace SampleApi.Tests.Helpers;

public static class TestHelpers
{
    public static User CreateTestUser(int id = 1, string email = "test@example.com", string password = "Test123!")
    {
        return new User
        {
            Id = id,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            FullName = "Test User",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static LoginRequest CreateLoginRequest(string email = "test@example.com", string password = "Test123!")
    {
        return new LoginRequest
        {
            Email = email,
            Password = password
        };
    }

    public static SignUpRequest CreateSignUpRequest(string email = "newuser@example.com", string password = "NewPass123!", string fullName = "New User")
    {
        return new SignUpRequest
        {
            Email = email,
            Password = password,
            FullName = fullName
        };
    }

    public static IConfiguration CreateTestConfiguration()
    {
        var configuration = new Dictionary<string, string?>
        {
            { "JwtSettings:SecretKey", "ThisIsAVeryLongSecretKeyForTestingPurposesOnly123456789" },
            { "JwtSettings:Issuer", "TestIssuer" },
            { "JwtSettings:Audience", "TestAudience" },
            { "JwtSettings:ExpiryMinutes", "60" }
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configuration)
            .Build();
    }

    public static IConfiguration CreateConfigurationWithMissingSecretKey()
    {
        var configuration = new Dictionary<string, string?>
        {
            { "JwtSettings:Issuer", "TestIssuer" },
            { "JwtSettings:Audience", "TestAudience" },
            { "JwtSettings:ExpiryMinutes", "60" }
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configuration)
            .Build();
    }
}

