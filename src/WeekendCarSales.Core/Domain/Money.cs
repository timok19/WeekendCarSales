using WeekendCarSales.Core.Common;

namespace WeekendCarSales.Core.Domain;

public readonly record struct Money
{
    public decimal Amount { get; }

    public Money(decimal amount)
    {
        if (amount < 0)
            throw new DomainException("Money amount cannot be negative.");

        Amount = decimal.Round(amount, 2);
    }

    public static Money operator +(Money left, Money right) => new(left.Amount + right.Amount);
}
