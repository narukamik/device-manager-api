namespace device_manager_api.Application.DTOs.Responses;

/// <summary>
/// Paginated response wrapper for collections
/// </summary>
/// <typeparam name="T">Type of items in the collection</typeparam>
public record PaginatedResponse<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages,
    bool HasNext,
    bool HasPrevious
);
