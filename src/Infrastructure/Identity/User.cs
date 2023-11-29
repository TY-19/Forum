using Forum.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Forum.Infrastructure.Identity;

public class User : IdentityUser, IUser
{
}
