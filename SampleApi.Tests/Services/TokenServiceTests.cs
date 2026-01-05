using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using SampleApi.Entities;
using SampleApi.Services;
using SampleApi.Tests.Helpers;

namespace SampleApi.Tests.Services;

public class TokenServiceTests
{
    [Fact]
    public void GenerateToken_WithValidUser_ReturnsValidJwtToken()
    {
        // Arrange
        var configuration = TestHelpers.CreateTestConfiguration();
        var tokenService = new TokenService(configuration);
        var user = TestHelpers.CreateTestUser();

        // Act
        var token = tokenService.GenerateToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Should().Contain(".");
        var parts = token.Split('.');
        parts.Should().HaveCount(3); // JWT has 3 parts: header.payload.signature
    }

    [Fact]
    public void GenerateToken_WithValidUser_ContainsCorrectClaims()
    {
        // Arrange
        var configuration = TestHelpers.CreateTestConfiguration();
        var tokenService = new TokenService(configuration);
        var user = TestHelpers.CreateTestUser(id: 123, email: "testuser@example.com", password: "Test123!");

        // Act
        var token = tokenService.GenerateToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        
        // Decode token to verify claims
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        
        jsonToken.Claims.Should().Contain(c => c.Type == "sub" && c.Value == "123");
        jsonToken.Claims.Should().Contain(c => c.Type == "email" && c.Value == "testuser@example.com");
        jsonToken.Claims.Should().Contain(c => c.Type == "name" && c.Value == "Test User");
        jsonToken.Claims.Should().Contain(c => c.Type == "jti");
        jsonToken.Claims.Should().Contain(c => c.Type == "iat");
    }

    [Fact]
    public void GenerateToken_WithValidUser_HasCorrectExpiration()
    {
        // Arrange
        var configuration = TestHelpers.CreateTestConfiguration();
        var tokenService = new TokenService(configuration);
        var user = TestHelpers.CreateTestUser();

        // Act
        var token = tokenService.GenerateToken(user);

        // Assert
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        
        jsonToken.ValidTo.Should().BeAfter(DateTime.UtcNow);
        jsonToken.ValidTo.Should().BeBefore(DateTime.UtcNow.AddHours(2)); // Should expire within 60 minutes + buffer
    }

    [Fact]
    public void GenerateToken_WithValidUser_HasCorrectIssuerAndAudience()
    {
        // Arrange
        var configuration = TestHelpers.CreateTestConfiguration();
        var tokenService = new TokenService(configuration);
        var user = TestHelpers.CreateTestUser();

        // Act
        var token = tokenService.GenerateToken(user);

        // Assert
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        
        jsonToken.Issuer.Should().Be("TestIssuer");
        jsonToken.Audiences.Should().Contain("TestAudience");
    }

    [Fact]
    public void GenerateToken_WithMissingSecretKey_ThrowsInvalidOperationException()
    {
        // Arrange
        var configuration = TestHelpers.CreateConfigurationWithMissingSecretKey();
        var tokenService = new TokenService(configuration);
        var user = TestHelpers.CreateTestUser();

        // Act & Assert
        var act = () => tokenService.GenerateToken(user);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("JWT SecretKey is not configured");
    }

    [Fact]
    public void GenerateToken_WithDifferentUsers_GeneratesDifferentTokens()
    {
        // Arrange
        var configuration = TestHelpers.CreateTestConfiguration();
        var tokenService = new TokenService(configuration);
        var user1 = TestHelpers.CreateTestUser(id: 1, email: "user1@example.com");
        var user2 = TestHelpers.CreateTestUser(id: 2, email: "user2@example.com");

        // Act
        var token1 = tokenService.GenerateToken(user1);
        var token2 = tokenService.GenerateToken(user2);

        // Assert
        token1.Should().NotBe(token2);
    }
}

