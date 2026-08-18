using FinanceHub.Domain.Entities;
using FluentAssertions;

namespace FinanceHub.Tests.Domain;

public class ProcessedEventTests
{
    [Fact]
    public void Construtor_DeveCriarProcessedEventComDadosInformados()
    {
        // Arrange

        var eventId = Guid.NewGuid();
        var eventType = "CompraCriadaEvent";

        // Act

        var processedEvent = new ProcessedEvent(
            eventId,
            eventType);

        // Assert

        processedEvent.Id.Should().NotBeEmpty();

        processedEvent.EventId.Should().Be(eventId);

        processedEvent.EventType.Should().Be(eventType);

        processedEvent.ProcessedAt
            .Should()
            .BeCloseTo(
                DateTime.UtcNow,
                TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Construtor_EventIdVazio_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new ProcessedEvent(
            Guid.Empty,
            "CompraCriadaEvent");

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("eventId");
    }

    [Fact]
    public void Construtor_EventTypeVazio_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new ProcessedEvent(
            Guid.NewGuid(),
            "");

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("eventType");
    }

    [Fact]
    public void Construtor_EventTypeSomenteEspacos_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new ProcessedEvent(
            Guid.NewGuid(),
            "   ");

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("eventType");
    }
}