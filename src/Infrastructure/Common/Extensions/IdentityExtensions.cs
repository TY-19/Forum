using Microsoft.AspNetCore.Identity;

namespace Forum.Infrastructure.Common.Extensions;

public static class IdentityExtensions
{
    public static string ToErrorString(this IEnumerable<IdentityError> errors)
    {
        if (errors == null) return string.Empty;
        return string.Join("\n", errors.Select(e => e.Description));
    }
}
