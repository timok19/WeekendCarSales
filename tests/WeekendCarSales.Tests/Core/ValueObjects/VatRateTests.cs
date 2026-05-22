using Shouldly;
using WeekendCarSales.Core.Common;
using WeekendCarSales.Core.Domain;
using Xunit;

namespace WeekendCarSales.Tests.Core.ValueObjects;

public sealed class VatRateTests
{
    [Fact]
    public void Constructor_SetsPercent()
    {
        var vat = new VatRate(20m);
        vat.Percent.ShouldBe(20m);
    }

    [Fact]
    public void Constructor_RoundsToTwoDecimalPlaces()
    {
        var vat = new VatRate(19.999m);
        vat.Percent.ShouldBe(20.00m);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(100.01)]
    public void Constructor_ThrowsDomainException_WhenPercentIsOutOfRange(decimal percent)
    {
        Should.Throw<DomainException>(() => new VatRate(percent));
    }

    [Fact]
    public void Multiplier_ReturnsCorrectValue()
    {
        var vat = new VatRate(20m);
        vat.Multiplier.ShouldBe(1.2m);
    }
}
