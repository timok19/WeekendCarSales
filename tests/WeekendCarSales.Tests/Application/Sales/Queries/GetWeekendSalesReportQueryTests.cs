using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Shouldly;
using WeekendCarSales.Application.Abstractions;
using WeekendCarSales.Application.Sales.Queries;
using WeekendCarSales.Core.Domain;
using Xunit;

namespace WeekendCarSales.Tests.Application.Sales.Queries;

public sealed class GetWeekendSalesReportQueryTests
{
    private readonly ICarSaleRepository _repository = Substitute.For<ICarSaleRepository>();
    private readonly GetWeekendSalesReportQuery _query;

    public GetWeekendSalesReportQueryTests()
    {
        _query = new GetWeekendSalesReportQuery(_repository, NullLogger<GetWeekendSalesReportQuery>.Instance);
    }

    [Fact]
    public async Task Handle_GroupsAndSummarizesSales()
    {
        var sales = new List<CarSale>
        {
            new("Model A", new DateTime(2010, 12, 4), new Money(100), new VatRate(20)), // Sat
            new("Model A", new DateTime(2010, 12, 5), new Money(200), new VatRate(20)), // Sun
            new("Model B", new DateTime(2010, 12, 4), new Money(500), new VatRate(20)), // Sat
        };
        _repository.GetWeekendSales(Arg.Any<CancellationToken>()).Returns(sales);

        var result = await _query.Handle();

        result.IsSuccess.ShouldBeTrue();
        result.Value.Count.ShouldBe(2);

        var modelA = result.Value.First(x => x.ModelName == "Model A");
        modelA.TotalWithoutVat.ShouldBe(300m);
        modelA.TotalWithVat.ShouldBe(360m);
        modelA.WeekendSalesCount.ShouldBe(2);

        var modelB = result.Value.First(x => x.ModelName == "Model B");
        modelB.TotalWithoutVat.ShouldBe(500m);
        modelB.TotalWithVat.ShouldBe(600m);
        modelB.WeekendSalesCount.ShouldBe(1);
    }
}
