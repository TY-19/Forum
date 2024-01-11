using Forum.Application.Common.Interfaces;
using Forum.Application.Search.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Forum.Application.Common.Helpers;

public class SearchHelper<T> : ISearchHelper<T>
    where T : new()
{
    private const int multiplier = 3;
    private const int maxPageSize = 100;
    private const int defaultPageSize = 10;

    private int pageSize;

    private bool hasNextPage = true;
    private int skipNumberElements;

    private Func<T, string>? findByOneField;
    private Func<T, IEnumerable<string>>? findByMultipleFields;

    private string[] searchWords = [];
    private Func<string, bool> searchFunction = (word) => false;


    public async Task<SearchResult<T>> SearchAsync(string search, IQueryable<T> collection, Func<T, string> findBy,
        SearchParameters? parameters, CancellationToken cancellationToken)
    {
        findByOneField = findBy;
        return await SearchAsync(search, collection, parameters, cancellationToken);
    }

    public async Task<SearchResult<T>> SearchAsync(string search, IQueryable<T> collection, Func<T, IEnumerable<string>> findBy,
        SearchParameters? parameters, CancellationToken cancellationToken)
    {
        findByMultipleFields = findBy;
        return await SearchAsync(search, collection, parameters, cancellationToken);
    }

    private async Task<SearchResult<T>> SearchAsync(string search, IQueryable<T> collection,
        SearchParameters? parameters, CancellationToken cancellationToken)
    {
        searchWords = search.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        SetPageParameters(parameters?.SkipNumberElements, parameters?.PageSize);
        SetSearchFunction(parameters?.SearchExact, parameters?.SearchAllWords, parameters?.CustomSearchFunction);
        return GetSearchResult(await FilterAsync(collection, cancellationToken));
    }

    private void SetPageParameters(int? skipElements, int? takeElements)
    {
        skipNumberElements = skipElements ?? 0;
        if (takeElements == null)
            pageSize = defaultPageSize;
        else if (takeElements > maxPageSize)
            pageSize = maxPageSize;
        else pageSize = takeElements.Value;
    }

    private void SetSearchFunction(bool? exact, bool? allWords, Func<string, bool>? custom)
    {
        if (custom != null)
        {
            searchFunction = custom;
        }
        else if (exact == true)
        {
            const string pattern = @"\s*(?:<[^<>]+>\s*)*";
            string lookFor = string.Join(pattern, searchWords);
            var regex = new Regex(lookFor, RegexOptions.IgnoreCase);
            searchFunction = regex.IsMatch;
        }
        else if (allWords == true)
        {
            searchFunction = (text) => Array.TrueForAll(searchWords, word => text.Contains(word, StringComparison.InvariantCultureIgnoreCase));
        }
        else
        {
            searchFunction = (text) => Array.Exists(searchWords, word => text.Contains(word, StringComparison.InvariantCultureIgnoreCase));
        }
    }

    private async Task<List<T>> FilterAsync(IQueryable<T> collection, CancellationToken cancellationToken)
    {
        var found = new List<T>();

        while (found.Count < pageSize)
        {
            var toChecks = await collection
                .Skip(skipNumberElements)
                .Take(multiplier * pageSize)
                .ToListAsync(cancellationToken);

            if (toChecks.Count == 0)
            {
                hasNextPage = false;
                break;
            }

            foreach (var toCheck in toChecks)
            {
                skipNumberElements++;

                if (!IsInSearch(toCheck))
                    continue;

                found.Add(toCheck);

                if (found.Count == pageSize)
                    break;
            }
        }
        return found;
    }

    private bool IsInSearch(T toCheck)
    {
        if (findByOneField != null)
        {
            return searchFunction(findByOneField(toCheck));
        }
        else if (findByMultipleFields != null)
        {
            return findByMultipleFields(toCheck).Any(x => searchFunction(x));
        }
        else return false;
    }

    private SearchResult<T> GetSearchResult(List<T> found)
    {
        return new SearchResult<T>()
        {
            Elements = found,
            HasNextPage = hasNextPage,
            SkipElementsForNextPage = hasNextPage ? skipNumberElements : null
        };
    }
}
