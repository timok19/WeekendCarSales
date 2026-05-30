using FluentResults;

namespace WeekendCarSales.Presentation.Extensions;

internal static class ResultExtensions
{
    public static string ToErrorMessage(this ResultBase result) =>
        string.Join(Environment.NewLine, result.Errors.Select(error => error.Message));
}
