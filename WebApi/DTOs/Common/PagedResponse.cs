namespace WebApi.DTOs.Common;

public class PagedResponse<T>
{
    public IReadOnlyList<T> Data { get; init; } = [];
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
    public int TotalRecords { get; init; }
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;

    public static PagedResponse<T> Create(IReadOnlyList<T> data, int pageNumber, int pageSize, int totalRecords)
    {
        return new PagedResponse<T>
        {
            Data = data,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
        };
    }
}
