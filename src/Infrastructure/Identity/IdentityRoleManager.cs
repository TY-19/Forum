using Forum.Application.Common.Interfaces.Identyty;
using Microsoft.AspNetCore.Identity;

namespace Forum.Infrastructure.Identity;

public class IdentityRoleManager(RoleManager<IdentityRole> roleManager) : IRoleManager
{

}
