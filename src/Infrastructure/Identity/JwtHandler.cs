using Forum.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Forum.Infrastructure.Identity;

public class JwtHandler(IConfiguration configuration, IUserManager userManager) : IJwtHandler
{
    public async Task<JwtSecurityToken> GetTokenAsync(IUser user)
    {
        return new JwtSecurityToken(
            issuer: configuration?["JwtSettings:Issuer"] ?? "Forum",
            audience: configuration?["JwtSettings:Audience"] ?? "*",
            claims: await GetClaimsAsync(user),
            expires: GetExpirationTime(),
            signingCredentials: GetSigningCredentials()
            );
    }

    private async Task<IEnumerable<Claim>> GetClaimsAsync(IUser user)
    {
        var claims = new List<Claim>();
        if (user == null)
            return claims;

        claims.Add(new Claim(ClaimTypes.Name, user.UserName ?? string.Empty));
        claims.Add(new Claim(ClaimTypes.Email, user.Email ?? string.Empty));

        foreach (string role in await userManager.GetRolesAsync(user, new CancellationToken()))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        return claims;
    }

    private DateTime GetExpirationTime()
    {
        double expiresIn = double.TryParse(configuration?["JwtSettings:ExpirationTimeInMinutes"], out double exp) ? exp : 60.0;
        return DateTime.Now.AddMinutes(expiresIn);
    }

    private SigningCredentials GetSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(configuration?["JwtSettings:SecurityKey"] ?? "defaultKey_that_is_32_characters");
        var secret = new SymmetricSecurityKey(key);
        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }
}
