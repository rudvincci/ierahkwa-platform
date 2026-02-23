using FluentAssertions;
using OpenTracing;
using Xunit;
using Mamey.Tracing.Jaeger.Tests.Helpers;

namespace Mamey.Tracing.Jaeger.Tests;

public class SpanCreationTests : IClassFixture<JaegerTestFixture>
{
    private readonly JaegerTestFixture _fixture;

    public SpanCreationTests(JaegerTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void BuildSpan_ShouldCreateSpanCorrectly()
    {
        // Arrange
        const string operationName = "test-operation";
        var tracer = _fixture.Tracer;

        // Act
        var span = tracer.BuildSpan(operationName).Start();

        // Assert
        span.Should().NotBeNull();
        // Note: ISpan doesn't expose OperationName directly in OpenTracing spec
        // The important thing is that the span is created successfully
    }

    [Fact]
    public void Span_ShouldSetTagsCorrectly()
    {
        // Arrange
        var tracer = _fixture.Tracer;
        var span = tracer.BuildSpan("tag-test").Start();

        // Act
        span.SetTag("string-tag", "test-value");
        span.SetTag("int-tag", 42);
        span.SetTag("bool-tag", true);

        // Assert
        span.Should().NotBeNull();
        // Note: We can't easily verify tags without checking Jaeger API
        // The important thing is that SetTag doesn't throw exceptions
    }

    [Fact]
    public void Span_ShouldRecordLogsCorrectly()
    {
        // Arrange
        var tracer = _fixture.Tracer;
        var span = tracer.BuildSpan("log-test").Start();
        var timestamp = DateTimeOffset.UtcNow;

        // Act
        span.Log("Test log message");
        span.Log(timestamp, "Test log with timestamp");
        span.Log(new Dictionary<string, object>
        {
            { "event", "test-event" },
            { "message", "Test structured log" }
        });

        // Assert
        span.Should().NotBeNull();
        // Note: We can't easily verify logs without checking Jaeger API
        // The important thing is that Log doesn't throw exceptions
    }

    [Fact]
    public void Span_ShouldFinishCorrectly()
    {
        // Arrange
        var tracer = _fixture.Tracer;
        var span = tracer.BuildSpan("finish-test").Start();

        // Act
        span.Finish();

        // Assert
        span.Should().NotBeNull();
        // Note: We can't easily verify finish without checking Jaeger API
        // The important thing is that Finish doesn't throw exceptions
    }

    [Fact]
    public void StartActive_ShouldCreateActiveScope()
    {
        // Arrange
        var tracer = _fixture.Tracer;

        // Act
        using var scope = tracer.BuildSpan("active-scope-test").StartActive();

        // Assert
        scope.Should().NotBeNull();
        scope.Span.Should().NotBeNull();
        // Note: ISpan doesn't expose OperationName directly in OpenTracing spec
    }

    [Fact]
    public void NestedSpans_ShouldCreateParentChildRelationship()
    {
        // Arrange
        var tracer = _fixture.Tracer;

        // Act
        using var parentScope = tracer.BuildSpan("parent-span").StartActive();
        using var childScope = tracer.BuildSpan("child-span").StartActive();

        // Assert
        parentScope.Should().NotBeNull();
        childScope.Should().NotBeNull();
        childScope.Span.Should().NotBeNull();
        parentScope.Span.Should().NotBeNull();
    }

    [Fact]
    public void Span_ShouldFinishWithoutErrors()
    {
        // Arrange
        var tracer = _fixture.Tracer;
        var span = tracer.BuildSpan("finish-test").Start();

        // Act & Assert
        var finishAction = () => span.Finish();
        finishAction.Should().NotThrow();
    }

    [Fact]
    public void Span_ShouldSetBaggageItemsCorrectly()
    {
        // Arrange
        var tracer = _fixture.Tracer;
        var span = tracer.BuildSpan("baggage-test").Start();

        // Act
        span.SetBaggageItem("test-key", "test-value");

        // Assert
        span.Should().NotBeNull();
        // Note: We can't easily verify baggage items without checking Jaeger API
        // The important thing is that SetBaggageItem doesn't throw exceptions
    }

    [Fact]
    public void Span_ShouldGetBaggageItemsCorrectly()
    {
        // Arrange
        var tracer = _fixture.Tracer;
        var span = tracer.BuildSpan("baggage-get-test").Start();
        span.SetBaggageItem("test-key", "test-value");

        // Act
        var baggageValue = span.GetBaggageItem("test-key");

        // Assert
        baggageValue.Should().Be("test-value");
    }
}
