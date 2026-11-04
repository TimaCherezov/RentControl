namespace Main.Model.Repositories;

public class PageList<T> : List<T>
{
    public int PageNumber { get; }
    public int TotalPages { get; }
    public int PageSize { get; }
    public long TotalCount { get; }

    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;

    public PageList(List<T> items, long totalCount, int pageNumber, int pageSize)
    {
        TotalCount = totalCount;
        PageSize = pageSize;
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        AddRange(items);
    }
}
