using TechStore.Domain.Entities;

namespace TechStore.BLL.Auth;

public interface IJwtService
{

    public Task<string> GenerateTokenAsync(ApplicationUser user);
}

