using Forum.Domain.Enums;
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

        if (!Enum.TryParse(policyName[PolicyPrefix.Length..], out PermissionType type))
            return await base.GetPolicyAsync(policyName);

        return new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(type))
            .Build();
    }
}
