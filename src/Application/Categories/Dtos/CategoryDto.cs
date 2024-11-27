using Forum.Domain.Entities;

namespace Forum.Application.Categories.Dtos;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentForumId { get; set; }
    public int Position { get; set; }

    public CategoryDto()
    {
    }

    public CategoryDto(Category category)
    {
        Id = category.Id;
        Name = category.Name;
        ParentForumId = category.ParentForumId;
        Position = category.Position;
    }
}