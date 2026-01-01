# Sample API - .NET 8.0 with SQL Server and JWT Authentication

A sample REST API project using .NET 8.0 with SQL Server (Docker) and JWT authentication.

## Features

- JWT Authentication (Login API)
- User Management APIs (Get list, Get detail)
- Entity Framework Core with SQL Server
- Swagger/OpenAPI documentation with JWT support

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started)

## Getting Started

### 1. Start SQL Server Container

```bash
docker-compose up -d
```

### 2. Run the API

```bash
cd SampleApi
dotnet run
```

The API will be available at:
- Swagger UI: http://localhost:5000/swagger
- API Base URL: http://localhost:5000

## API Endpoints

### Authentication

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/login` | Login with email/password | No |

### Users

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/users` | Get list of users (paginated) | Yes |
| GET | `/api/users/{id}` | Get user by ID | Yes |

## Sample Credentials

| Email | Password |
|-------|----------|
| admin@example.com | Admin@123 |
| user1@example.com | User1@123 |
| user2@example.com | User2@123 |

## Usage Examples

### Login

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "admin@example.com", "password": "Admin@123"}'
```

### Get Users (with token)

```bash
curl http://localhost:5000/api/users \
  -H "Authorization: Bearer <your-jwt-token>"
```

### Get User Detail

```bash
curl http://localhost:5000/api/users/1 \
  -H "Authorization: Bearer <your-jwt-token>"
```

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
