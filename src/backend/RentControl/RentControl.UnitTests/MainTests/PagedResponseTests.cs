using RentControl.Common.DTOs;

namespace RentControl.UnitTests.MainTests;

public class PagedResponseTests
{
    [Fact]
    public void TotalPages_WhenTotalNotDivisibleByPageSize_RoundsUp()
    {
        var response = new PagedResponse<string>([], TotalCount: 5, PageNumber: 1, PageSize: 2);

        Assert.Equal(3, response.TotalPages);
    }

    [Fact]
    public void HasPrevious_WhenPageIsFirst_IsFalse()
    {
        var response = new PagedResponse<string>([], TotalCount: 5, PageNumber: 1, PageSize: 2);

        Assert.False(response.HasPrevious);
        Assert.True(response.HasNext);
    }

    [Fact]
    public void HasNext_WhenPageIsLast_IsFalse()
    {
        var response = new PagedResponse<string>([], TotalCount: 5, PageNumber: 3, PageSize: 2);

        Assert.True(response.HasPrevious);
        Assert.False(response.HasNext);
    }
}
