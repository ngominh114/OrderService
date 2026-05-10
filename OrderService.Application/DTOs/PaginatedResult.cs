namespace OrderService.Application.DTOs;

/// <summary>
/// Generic paginated result wrapper containing data and pagination metadata.
/// </summary>
/// <typeparam name="T">Type of data items in the result</typeparam>
public class PaginatedResult<T>
{
    /// <summary>
    /// The items for the current page.
    /// </summary>
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// Total number of items across all pages.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Current page number (1-based).
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Number of items per page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of pages.
    /// </summary>
    public int TotalPages { get; set; }

    public PaginatedResult() { }

    public PaginatedResult(List<T> items, int totalCount, int page, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}

