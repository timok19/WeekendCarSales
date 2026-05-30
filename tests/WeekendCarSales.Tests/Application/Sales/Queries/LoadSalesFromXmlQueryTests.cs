using FluentResults;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Shouldly;
using WeekendCarSales.Application.Abstractions;
using WeekendCarSales.Application.Sales.Queries;
using WeekendCarSales.Core.Domain;
using Xunit;

namespace WeekendCarSales.Tests.Application.Sales.Queries;

public sealed class LoadSalesFromXmlQueryTests
{
    private readonly ISalesXmlImporter _xmlImporter = Substitute.For<ISalesXmlImporter>();
    private readonly LoadSalesFromXmlQuery _query;

    public LoadSalesFromXmlQueryTests()
    {
        _query = new LoadSalesFromXmlQuery(_xmlImporter, NullLogger<LoadSalesFromXmlQuery>.Instance);
    }

    [Fact]
    public async Task Handle_ReturnsFail_WhenXmlImporterFails()
    {
        _xmlImporter.LoadAsync(Arg.Any<string>()).Returns(Result.Fail<IReadOnlyList<CarSale>>("Error"));

        var result = await _query.Handle("test.xml");

        result.IsFailed.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_ReturnsDtos_WhenXmlImporterSucceeds()
    {
        var sales = new List<CarSale>
        {
            new(
                "Model A",
                new DateTime(2023, 10, 1),
                new Money(100),
                new VatRate(20)
            ),
        };

        _xmlImporter.LoadAsync(Arg.Any<string>()).Returns(Result.Ok<IReadOnlyList<CarSale>>(sales));

        var result = await _query.Handle("test.xml");

        result.IsSuccess.ShouldBeTrue();
        result.Value.Count.ShouldBe(1);
        result.Value[0].ModelName.ShouldBe("Model A");
        result.Value[0].PriceWithVat.ShouldBe(120m);
    }
}
