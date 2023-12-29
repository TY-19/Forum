using Forum.Domain.Common;

namespace Forum.Domain.Entities;

public class ApplicationRole : BaseEntity
{
    public string IdentityRoleId { get; set; } = null!;
    public IEnumerable<Permission> Permissions { get; set; } = new List<Permission>();
}
