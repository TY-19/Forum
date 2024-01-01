using Forum.Domain.Entities;

namespace Forum.Application.Common.Interfaces;

public interface IRole
{
    string Id { get; }
    string? Name { get; }
    public ApplicationRole ApplicationRole { get; set; }
}
