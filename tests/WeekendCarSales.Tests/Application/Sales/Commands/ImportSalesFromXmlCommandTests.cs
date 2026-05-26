using FluentResults;
using NSubstitute;
using Shouldly;
using WeekendCarSales.Application.Abstractions;
using WeekendCarSales.Application.Sales.Commands;
using WeekendCarSales.Core.Domain;
using Xunit;

namespace WeekendCarSales.Tests.Application.Sales.Commands;

public sealed class ImportSalesFromXmlCommandTests
{
    private readonly ISalesXmlImporter _xmlImporter = Substitute.For<ISalesXmlImporter>();
    private readonly ICarSaleRepository _repository = Substitute.For<ICarSaleRepository>();
    private readonly ImportSalesFromXmlCommand _sut;

    public ImportSalesFromXmlCommandTests()
    {
        _sut = new ImportSalesFromXmlCommand(_xmlImporter, _repository);
    }

    [Fact]
    public async Task Handle_ReturnsFail_WhenFilePathIsEmpty()
    {
        var result = await _sut.Handle("");

        result.IsFailed.ShouldBeTrue();
        result.Errors.First().Message.ShouldBe("Please select a valid XML file.");
    }

    [Fact]
    public async Task Handle_ReturnsFail_WhenXmlImporterFails()
    {
        _xmlImporter.LoadAsync(Arg.Any<string>()).Returns(Result.Fail<IReadOnlyList<CarSale>>("Error"));

        var result = await _sut.Handle("test.xml");

        result.IsFailed.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_ImportsData_WhenXmlImporterSucceeds()
    {
        var sales = new List<CarSale> { new("Model A", DateTime.Now, new Money(100), new VatRate(20)) };
        _xmlImporter.LoadAsync(Arg.Any<string>()).Returns(Result.Ok<IReadOnlyList<CarSale>>(sales));

        var result = await _sut.Handle("test.xml");

        result.IsSuccess.ShouldBeTrue();
        result.Value.ImportedCount.ShouldBe(1);
        await _repository.Received(1).ReplaceAll(sales, Arg.Any<CancellationToken>());
    }
}
