using Shouldly;
using WeekendCarSales.Core.Common;
using WeekendCarSales.Core.Domain;
using Xunit;

namespace WeekendCarSales.Tests.Core.ValueObjects;

public sealed class MoneyTests
{
    [Fact]
    public void Constructor_SetsAmount()
    {
        var money = new Money(100.55m);
        money.Amount.ShouldBe(100.55m);
    }

    [Fact]
    public void Constructor_RoundsToTwoDecimalPlaces()
    {
        var money = new Money(100.555m);
        money.Amount.ShouldBe(100.56m);
    }

    [Fact]
    public void Constructor_ThrowsDomainException_WhenAmountIsNegative()
    {
        Should.Throw<DomainException>(() => new Money(-10m));
    }

    [Fact]
    public void AdditionOperator_SumsAmountsCorrectly()
    {
        var m1 = new Money(10.50m);
        var m2 = new Money(20.25m);
        var sum = m1 + m2;
        sum.Amount.ShouldBe(30.75m);
    }
}
