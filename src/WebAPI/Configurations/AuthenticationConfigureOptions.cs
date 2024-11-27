using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Forum.WebAPI.Configurations;

public class AuthenticationConfigureOptions : IConfigureOptions<AuthenticationOptions>
{
    public void Configure(AuthenticationOptions options)
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }
}
