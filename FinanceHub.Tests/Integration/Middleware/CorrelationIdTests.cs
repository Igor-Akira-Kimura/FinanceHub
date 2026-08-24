using FinanceHub.Tests.Fixtures;
using FluentAssertions;
using System.Net;

namespace FinanceHub.Tests.Integration.Middleware;

public class CorrelationIdTests
    : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly CustomWebApplicationFactory _factory;

    public CorrelationIdTests(
        DatabaseFixture fixture)
    {
        _fixture = fixture;

        _fixture.ResetDatabase();

        _factory =
            new CustomWebApplicationFactory(
                _fixture.ConnectionString);
    }

    [Fact]
    public async Task Health_SemCorrelationId_DeveGerarCorrelationId()
    {
        // Arrange

        using var client =
            _factory.CreateClient();

        // Act

        var response =
            await client.GetAsync("/health");

        // Assert

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        response.Headers
            .Contains("X-Correlation-ID")
            .Should()
            .BeTrue();

        var correlationId =
            response.Headers
                .GetValues("X-Correlation-ID")
                .Single();

        correlationId
            .Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Health_ComCorrelationId_DeveManterMesmoId()
    {
        // Arrange

        using var client =
            _factory.CreateClient();

        var correlationId =
            "teste-correlation-123";

        client.DefaultRequestHeaders.Add(
            "X-Correlation-ID",
            correlationId);

        // Act

        var response =
            await client.GetAsync("/health");

        // Assert

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var responseCorrelationId =
            response.Headers
                .GetValues("X-Correlation-ID")
                .Single();

        responseCorrelationId
            .Should()
            .Be(correlationId);
    }
}