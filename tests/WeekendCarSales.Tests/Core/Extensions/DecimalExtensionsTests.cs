using WeekendCarSales.Core.Extensions;
using Shouldly;
using Xunit;

namespace WeekendCarSales.Tests.Core.Extensions;

public sealed class DecimalExtensionsTests
{
    [Fact]
    public void ToCzechCurrency_FormatsCorrectly()
    {
        1234.56m.ToCzechCurrency().ShouldBe("1 234,56");
    }
}
