using SampleApi.Entities;

namespace SampleApi.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}
