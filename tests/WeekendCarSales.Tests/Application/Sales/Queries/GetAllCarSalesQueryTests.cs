using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Shouldly;
using WeekendCarSales.Application.Abstractions;
using WeekendCarSales.Application.Sales.Queries;
using WeekendCarSales.Core.Domain;
using Xunit;

namespace WeekendCarSales.Tests.Application.Sales.Queries;

public sealed class GetAllCarSalesQueryTests
{
    private readonly ICarSaleRepository _repository = Substitute.For<ICarSaleRepository>();
    private readonly GetAllCarSalesQuery _query;

    public GetAllCarSalesQueryTests()
    {
        _query = new GetAllCarSalesQuery(_repository, NullLogger<GetAllCarSalesQuery>.Instance);
    }

    [Fact]
    public async Task Handle_ReturnsOrderedDtos()
    {
        var sales = new List<CarSale>
        {
            new("Model B", new DateTime(2023, 10, 2), new Money(200), new VatRate(20)),
            new("Model A", new DateTime(2023, 10, 1), new Money(100), new VatRate(20)),
        };
        _repository.GetAll(Arg.Any<CancellationToken>()).Returns(sales);

        var result = await _query.Handle();

        result.IsSuccess.ShouldBeTrue();
        result.Value.Count.ShouldBe(2);
        result.Value[0].ModelName.ShouldBe("Model A");
        result.Value[1].ModelName.ShouldBe("Model B");
    }
}
