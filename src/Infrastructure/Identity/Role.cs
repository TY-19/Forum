using Forum.Application.Common.Interfaces;
using Forum.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Forum.Infrastructure.Identity;

public class Role : IdentityRole, IRole
{
    public ApplicationRole ApplicationRole { get; set; } = null!;
    public Role() : base()
    { }

    public Role(string roleName) : base(roleName)
    { }
}
