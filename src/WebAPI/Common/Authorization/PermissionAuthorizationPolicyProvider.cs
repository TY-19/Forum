using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Forum.WebAPI.Common.Authorization;

public class PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : DefaultAuthorizationPolicyProvider(options)
{
    internal const string PolicyPrefix = "PERMISSION_";

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (!policyName.StartsWith(PolicyPrefix, StringComparison.OrdinalIgnoreCase))
            return await base.GetPolicyAsync(policyName);

        string permission = policyName[PolicyPrefix.Length..];

        var requirement = new PermissionRequirement(permission);

        return new AuthorizationPolicyBuilder()
            .AddRequirements(requirement).Build();
    }
}
