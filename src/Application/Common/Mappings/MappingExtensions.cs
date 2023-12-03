using Forum.Application.Common.Models;
using Forum.Domain.Entities;

namespace Forum.Application.Common.Mappings;

public static class MappingExtensions
{
    public static ForumDto ToForumDto(this ForumEntity forum)
    {
        return new ForumDto()
        {
            Id = forum.Id,
            Name = forum.Name,
            ParentForumId = forum.ParentForumId,
            Category = forum.Category,
            Description = forum.Description,
            Subcategories = forum.Subforums.Select(f => f.Category).Distinct(),
            Subforums = forum.Subforums.ToSubforumsDto(),
            Topics = forum.Topics.ToTopicsDto()
        };
    }

    public static IEnumerable<ForumDto> ToForumsDto(this IEnumerable<ForumEntity> forums)
    {
        foreach (var forum in forums)
        {
            yield return forum.ToForumDto();
        }
    }

    public static IEnumerable<ForumDto> ToForumsDto(this IQueryable<ForumEntity> forums)
    {
        foreach (var forum in forums)
        {
            yield return forum.ToForumDto();
        }
    }

    public static SubforumDto ToSubforumDto(this ForumEntity forum)
    {
        return new SubforumDto()
        {
            Id = forum.Id,
            Name = forum.Name,
            ParentForumId = forum.ParentForumId,
            Category = forum.Category,
            Description = forum.Description,
            SubforumsCount = forum.Subforums?.Count() ?? 0,
            TopicsCount = forum.Topics?.Count() ?? 0
        };
    }

    public static IEnumerable<SubforumDto> ToSubforumsDto(this IEnumerable<ForumEntity> forums)
    {
        foreach (var forum in forums)
        {
            yield return forum.ToSubforumDto();
        }
    }
    public static TopicDto ToTopicDto(this Topic topic)
    {
        return new TopicDto()
        {
            TopicName = topic.TopicName,
            ParentForumId = topic.ParentForumId,
            MessagesCount = topic.Messages?.Count() ?? 0
        };
    }
    public static IEnumerable<TopicDto> ToTopicsDto(this IEnumerable<Topic> topics)
    {
        foreach (var topic in topics)
        {
            yield return topic.ToTopicDto();
        }
    }
}
