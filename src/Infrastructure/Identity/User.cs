﻿using Forum.Application.Common.Interfaces;
using Forum.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Forum.Infrastructure.Identity;

public class User : IdentityUser, IUser
{
    public UserProfile UserProfile { get; set; } = null!;
}
