using Main.Model.Repositories;

namespace RentControl.UnitTests.MainTests;

public class PageListTests
{
    [Fact]
    public void TotalPages_WhenTotalNotDivisibleByPageSize_RoundsUp()
    {
        var page = new PageList<int>([1, 2], totalCount: 5, pageNumber: 1, pageSize: 2);

        Assert.Equal(3, page.TotalPages);
    }

    [Fact]
    public void TotalPages_WhenTotalDivisibleByPageSize_IsExact()
    {
        var page = new PageList<int>([1, 2], totalCount: 4, pageNumber: 1, pageSize: 2);

        Assert.Equal(2, page.TotalPages);
    }

    [Fact]
    public void HasPrevious_WhenPageIsFirst_IsFalse()
    {
        var page = new PageList<int>([1, 2], totalCount: 5, pageNumber: 1, pageSize: 2);

        Assert.False(page.HasPrevious);
        Assert.True(page.HasNext);
    }

    [Fact]
    public void HasPreviousAndNext_WhenPageIsMiddle_AreTrue()
    {
        var page = new PageList<int>([3, 4], totalCount: 5, pageNumber: 2, pageSize: 2);

        Assert.True(page.HasPrevious);
        Assert.True(page.HasNext);
    }

    [Fact]
    public void HasNext_WhenPageIsLast_IsFalse()
    {
        var page = new PageList<int>([5], totalCount: 5, pageNumber: 3, pageSize: 2);

        Assert.True(page.HasPrevious);
        Assert.False(page.HasNext);
    }
}
