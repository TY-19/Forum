using Microsoft.AspNetCore.Identity;

namespace Forum.Infrastructure.Common.Extensions;

public static class IdentityExtensions
{
    public static string ToErrorString(this IEnumerable<IdentityError> errors)
        => errors == null
            ? string.Empty
            : string.Join("\n", errors.Select(e => e.Description));
}
