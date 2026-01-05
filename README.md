# Sample API - .NET 8.0 with SQL Server and JWT Authentication

A sample REST API project using .NET 8.0 with SQL Server (Docker) and JWT authentication.

## Project Structure

```
SampleApi/
├── Controllers/          # API Controllers
│   ├── AuthController.cs
│   └── UsersController.cs
├── Data/                 # Database Context
│   └── AppDbContext.cs
├── DTOs/                 # Data Transfer Objects
│   ├── LoginRequest.cs
│   ├── LoginResponse.cs
│   ├── UserDto.cs
│   └── UserListResponse.cs
├── Entities/             # Domain Entities
│   └── User.cs
├── Services/             # Business Services
│   ├── ITokenService.cs
│   └── TokenService.cs
├── Program.cs
└── appsettings.json
```

## Configuration

Edit `appsettings.json` to configure:

- **Connection String**: SQL Server connection
- **JWT Settings**: Secret key, issuer, audience, and token expiry time

## Running Tests

### Basic Test Command
```bash
dotnet test
```

### View Tests with Detailed Output
```bash
dotnet test --verbosity normal
```

### View Tests with Numbered Format (Recommended)
```bash
./run-tests.sh
```

Hoặc với các options khác:
```bash
./run-tests.sh --collect:"XPlat Code Coverage"
```

### Run Tests with Code Coverage
```bash
dotnet test --collect:"XPlat Code Coverage" --results-directory:"./TestResults" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura
```

### Test Output Format
Khi sử dụng `./run-tests.sh`, bạn sẽ thấy output dạng:
```
✓ Test-case 1: PASSED - SampleApi.Tests.Services.TokenServiceTests.GenerateToken_WithValidUser_ReturnsValidJwtToken
✓ Test-case 2: PASSED - SampleApi.Tests.Controllers.AuthControllerTests.Login_WithValidCredentials_ReturnsOkWithToken
✗ Test-case 3: FAILED - SampleApi.Tests.Controllers.AuthControllerTests.Login_WithInvalidEmail_ReturnsUnauthorized
```
