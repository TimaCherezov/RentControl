namespace RentControl.Common.DTOs;

public record PagedResponse<T>(
    IReadOnlyList<T> Items,
    long TotalCount,
    int PageNumber,
    int PageSize
)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;
}
