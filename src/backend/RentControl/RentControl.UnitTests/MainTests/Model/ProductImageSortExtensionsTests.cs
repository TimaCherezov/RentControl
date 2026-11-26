using Main.Model.Models;
using Main.Model.Repositories;

namespace RentControl.UnitTests.MainTests.Model;

public class ProductImageSortExtensionsTests
{
    private static readonly Guid FirstId = new("00000000-0000-0000-0000-000000000001");
    private static readonly Guid SecondId = new("00000000-0000-0000-0000-000000000002");
    private static readonly DateTimeOffset Earlier = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset Later = new(2026, 2, 1, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public void SortForDisplay_PutsMainImageFirst()
    {
        var plain = new ProductImageBuilder().WithId(FirstId).WithCreatedAt(Earlier).Build();
        var main = new ProductImageBuilder().WithId(SecondId).AsMain().WithCreatedAt(Later).Build();

        var sorted = new[] { plain, main }.SortForDisplay().ToList();

        Assert.Equal(main.Id, sorted[0].Id);
    }

    [Fact]
    public void SortForDisplay_OrdersNonMainImagesByCreatedAt()
    {
        var later = new ProductImageBuilder().WithId(FirstId).WithCreatedAt(Later).Build();
        var earlier = new ProductImageBuilder().WithId(SecondId).WithCreatedAt(Earlier).Build();

        var sorted = new[] { later, earlier }.SortForDisplay().ToList();

        Assert.Equal(earlier.Id, sorted[0].Id);
        Assert.Equal(later.Id, sorted[1].Id);
    }

    [Fact]
    public void SortForDisplay_WhenCreatedAtIsEqual_TiesByImageId()
    {
        var second = new ProductImageBuilder().WithId(SecondId).WithCreatedAt(Earlier).Build();
        var first = new ProductImageBuilder().WithId(FirstId).WithCreatedAt(Earlier).Build();

        var sorted = new[] { second, first }.SortForDisplay().ToList();

        Assert.Equal(FirstId, sorted[0].Id);
        Assert.Equal(SecondId, sorted[1].Id);
    }
}
