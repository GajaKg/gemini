using gemini.Utilities;

namespace Scrap.Tests.Utilities;

public class CurrencyNormalizeTest
{
    [Fact]
    public void ExtractNormalizeValueCultureFr_Should_Parse_French_Number()
    {
        // Arrange
        string input = "123,45";

        // Act
        var result = CurrencyNormalize.ExtractNormalizeValueCultureFr(input);

        // Assert
        Assert.Equal(123.45m, result);
    }

    [Fact]
    public void ExtractNormalizeValueCultureFr_Should_Return_Null_When_Invalid()
    {
        // Arrange
        string input = "abcd";

        // Act
        var result = CurrencyNormalize.ExtractNormalizeValueCultureFr(input);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ExtractNormalizeValueCultureFr_Should_Trim_Spaces()
    {
        var result = CurrencyNormalize.ExtractNormalizeValueCultureFr("   45,12   ");

        Assert.Equal(45.12m, result);
    }

    [Fact]
    public void ExtractNormalizeValueCultureFr_Should_Remove_Html_Entities()
    {
        var result = CurrencyNormalize.ExtractNormalizeValueCultureFr("&nbsp;12,50");

        Assert.Equal(12.50m, result);
    }

    [Fact]
    public void ExtractNormalizeValueCultureUs_Should_Parse_Us_Number()
    {
        var result = CurrencyNormalize.ExtractNormalizeValueCultureUs("123.45");

        Assert.Equal(123.45m, result);
    }

    [Fact]
    public void ExtractNormalizeValueCultureUs_Should_Return_Null_When_Invalid()
    {
        var result = CurrencyNormalize.ExtractNormalizeValueCultureUs("hello");

        Assert.Null(result);
    }
}
