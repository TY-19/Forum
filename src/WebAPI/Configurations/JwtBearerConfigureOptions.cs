using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Forum.WebAPI.Configurations;

public class JwtBearerConfigureOptions(IConfiguration configuration)
{
    public void Configure(JwtBearerOptions options)
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            RequireExpirationTime = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration?["JwtSettings:Issuer"] ?? "Forum",
            ValidAudience = configuration?["JwtSettings:Audience"] ?? "*",
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                    configuration?["JwtSettings:SecurityKey"] ?? "defaultKey_that_is_32_characters"))
        };
    }
}
