using Forum.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Forum.Infrastructure.Identity;

public class Role : IdentityRole
{
    public int ApplicationRoleId { get; set; }
    public ApplicationRole ApplicationRole { get; set; } = null!;
    public Role() : base()
    { }

    public Role(string roleName) : base(roleName)
    { }
}
