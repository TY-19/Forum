using System.IdentityModel.Tokens.Jwt;

namespace Forum.Application.Common.Interfaces;

public interface IJwtHandler
{
    Task<JwtSecurityToken> GetTokenAsync(IUser user);
}
