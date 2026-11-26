using Main.Api.Features.ProductImages;

namespace RentControl.UnitTests.MainTests;

public class ImageStorageOptionsTests
{
    [Fact]
    public void Build_WhenBaseUrlIsMissing_ThrowsInvalidOperationException()
    {
        var options = new ImageStorageOptions { ImageBaseUrl = "" };

        Assert.Throws<InvalidOperationException>(() => options.Build("images/photo.jpg"));
    }

    [Fact]
    public void Build_WhenBaseUrlIsSet_TrimsTrailingAndLeadingSlashes()
    {
        var options = new ImageStorageOptions { ImageBaseUrl = "http://images.test/" };

        var url = options.Build("/images/photo.jpg");

        Assert.Equal("http://images.test/images/photo.jpg", url);
    }
}
