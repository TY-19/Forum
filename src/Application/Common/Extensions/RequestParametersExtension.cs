using Forum.Application.Common.Models;

namespace Forum.Application.Common.Extensions;

public static class RequestParametersExtension
{
    public static void SetPageOptions(this RequestParameters requestParameters, int defaultPageSize, int? maxPageSize)
    {
        requestParameters.PageSize ??= defaultPageSize;
        if (maxPageSize != null && requestParameters.PageSize > maxPageSize)
        {
            requestParameters.PageSize = maxPageSize;
        }

        requestParameters.PageNumber ??= 1;
    }

    public static void SetPageOptions(this RequestParameters requestParameters, int defaultPageSize,
        int? maxPageSize, out int pageSize, out int pageNumber)
    {
        SetPageOptions(requestParameters, defaultPageSize, maxPageSize);
        pageSize = (int)requestParameters.PageSize!;
        pageNumber = (int)requestParameters.PageNumber!;
    }
}
