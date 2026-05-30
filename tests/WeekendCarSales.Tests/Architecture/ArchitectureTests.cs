using System.Reflection;
using NetArchTest.Rules;
using Shouldly;
using WeekendCarSales.Application.Abstractions;
using WeekendCarSales.Core.Domain;
using WeekendCarSales.Infrastructure.Repositories;
using Xunit;

namespace WeekendCarSales.Tests.Architecture;

public sealed class ArchitectureTests
{
    private const string Core = "WeekendCarSales.Core";
    private const string Application = "WeekendCarSales.Application";
    private const string Infrastructure = "WeekendCarSales.Infrastructure";
    private const string Presentation = "WeekendCarSales.Presentation";

    private static readonly Assembly CoreAssembly = typeof(CarSale).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(ICarSaleRepository).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(CarSaleRepository).Assembly;

    [Fact]
    public void Core_ShouldNotDependOnAnyOtherLayer()
    {
        var result = Types.InAssembly(CoreAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(Application, Infrastructure, Presentation)
            .GetResult();

        AssertSuccessful(result, "Core must not depend on Application, Infrastructure, or Presentation.");
    }

    [Fact]
    public void Application_ShouldNotDependOnInfrastructure()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOn(Infrastructure)
            .GetResult();

        AssertSuccessful(result, "Application must depend only on abstractions, not on Infrastructure.");
    }

    [Fact]
    public void Application_ShouldNotDependOnPresentation()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOn(Presentation)
            .GetResult();

        AssertSuccessful(result, "Application must not depend on Presentation.");
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOnPresentation()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOn(Presentation)
            .GetResult();

        AssertSuccessful(result, "Infrastructure must not depend on Presentation.");
    }

    [Fact]
    public void RepositoryImplementations_ShouldLiveInInfrastructure()
    {
        // Concrete repositories belong in Infrastructure; Application only sees the interfaces.
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("Repository")
            .Should()
            .BeInterfaces()
            .GetResult();

        AssertSuccessful(result, "Repository types in Application must be interfaces, not implementations.");
    }

    [Fact]
    public void DomainTypes_ShouldBeSealed()
    {
        // Domain entities/value objects are immutable records and not designed for inheritance.
        var result = Types.InAssembly(CoreAssembly)
            .That()
            .ResideInNamespace($"{Core}.Domain")
            .And()
            .AreClasses()
            .Should()
            .BeSealed()
            .GetResult();

        AssertSuccessful(result, "Domain classes should be sealed.");
    }

    private static void AssertSuccessful(TestResult result, string because)
    {
        var failing = result.FailingTypeNames is null ? string.Empty : string.Join(", ", result.FailingTypeNames);
        result.IsSuccessful.ShouldBeTrue($"{because} Offending types: {failing}");
    }
}
