using Forum.Application.Search.Dtos;

namespace Forum.Application.Common.Interfaces;

public interface ISearchHelper<T>
{
    Task<SearchResult<T>> SearchAsync(string search, IQueryable<T> collection, Func<T, string> findBy,
        SearchParameters? parameters, CancellationToken cancellationToken);

    Task<SearchResult<T>> SearchAsync(string search, IQueryable<T> collection, Func<T, IEnumerable<string>> findBy,
        SearchParameters? parameters, CancellationToken cancellationToken);
}
