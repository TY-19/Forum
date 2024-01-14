using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Forum.WebAPI.Configurations;

public class AuthorizationConfigureOptions : IConfigureOptions<AuthorizationOptions>
{
    public void Configure(AuthorizationOptions options)
    {
        options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .Build();
    }
}
