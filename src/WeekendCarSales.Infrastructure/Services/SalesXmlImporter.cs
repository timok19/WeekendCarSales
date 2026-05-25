using System.Globalization;
using System.Xml.Linq;
using FluentResults;
using WeekendCarSales.Application.Abstractions;
using WeekendCarSales.Core.Domain;

namespace WeekendCarSales.Infrastructure.Services;

public sealed class SalesXmlImporter : ISalesXmlImporter
{
    private static readonly CultureInfo CzechCulture = CultureInfo.GetCultureInfo("cs-CZ");
    private static readonly string[] SupportedDateFormats = ["d.M.yyyy", "dd.MM.yyyy", "yyyy-MM-dd"];

    public Task<Result<IReadOnlyList<CarSale>>> LoadAsync(string filePath)
    {
        if (!File.Exists(filePath))
            return Task.FromResult(Result.Fail<IReadOnlyList<CarSale>>($"XML file was not found: {filePath}"));

        try
        {
            var document = XDocument.Load(filePath);
            var sales = document.Root?.Elements("sale").Select(ParseSale).ToList();

            return sales is null || sales.Count == 0
                ? Task.FromResult(Result.Fail<IReadOnlyList<CarSale>>("The XML file does not contain any sale records."))
                : Task.FromResult(Result.Ok<IReadOnlyList<CarSale>>(sales));
        }
        catch (Exception ex) when (ex is FormatException or InvalidOperationException or System.Xml.XmlException)
        {
            return Task.FromResult(Result.Fail<IReadOnlyList<CarSale>>($"The selected XML file has an invalid structure: {ex.Message}"));
        }
    }

    private static CarSale ParseSale(XElement saleElement)
    {
        var modelName = RequiredValue(saleElement, "model");
        var soldOn = ParseDate(RequiredValue(saleElement, "soldOn"));
        var priceWithoutVat = ParseDecimal(RequiredValue(saleElement, "priceWithoutVat"));
        var vatRate = ParseDecimal(RequiredValue(saleElement, "vatRate"));

        return new CarSale(modelName, soldOn, new Money(priceWithoutVat), new VatRate(vatRate));
    }

    private static string RequiredValue(XElement saleElement, string elementName) =>
        saleElement.Element(elementName)?.Value ?? throw new InvalidOperationException($"Missing required element '{elementName}'.");

    private static DateTime ParseDate(string value) =>
        DateTime.TryParseExact(value, SupportedDateFormats, CzechCulture, DateTimeStyles.None, out var parsed)
            ? parsed.Date
            : throw new FormatException($"Cannot parse date '{value}'.");

    private static decimal ParseDecimal(string value)
    {
        var normalized = value
            .Trim()
            .Replace(" ", string.Empty)
            .Replace("Kč", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace(",-", string.Empty);

        if (normalized.Contains('.') && normalized.Contains(','))
            normalized = normalized.Replace(".", string.Empty).Replace(',', '.');
        else if (normalized.Contains(','))
            normalized = normalized.Replace(',', '.');
        else if (normalized.Count(character => character == '.') == 1 && normalized.Split('.')[1].Length == 3)
            normalized = normalized.Replace(".", string.Empty);

        return decimal.Parse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture);
    }
}
