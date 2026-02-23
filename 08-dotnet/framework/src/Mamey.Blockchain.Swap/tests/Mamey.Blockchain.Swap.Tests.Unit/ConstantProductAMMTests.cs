using FluentAssertions;
using Mamey.Blockchain.Swap;
using Xunit;

namespace Mamey.Blockchain.Swap.Tests.Unit;

public class ConstantProductAMMTests
{
    [Fact]
    public void CalculateOutput_WithValidInputs_ShouldReturnCorrectOutput()
    {
        // Arrange
        var amm = new ConstantProductAMM(feeRate: 0.003m);
        var amountIn = 1000m;
        var reserveIn = 1000000m;
        var reserveOut = 1000000m;

        // Act
        var output = amm.CalculateOutput(amountIn, reserveIn, reserveOut);

        // Assert
        output.Should().BeGreaterThan(0);
        output.Should().BeLessThan(amountIn); // Output should be less than input due to fees
    }

    [Fact]
    public void CalculateOutput_WithZeroReserves_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var amm = new ConstantProductAMM();
        var amountIn = 1000m;
        var reserveIn = 0m;
        var reserveOut = 1000000m;

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => 
            amm.CalculateOutput(amountIn, reserveIn, reserveOut));
    }

    [Fact]
    public void CalculateInput_WithValidOutput_ShouldReturnCorrectInput()
    {
        // Arrange
        var amm = new ConstantProductAMM(feeRate: 0.003m);
        var amountOut = 990m;
        var reserveIn = 1000000m;
        var reserveOut = 1000000m;

        // Act
        var input = amm.CalculateInput(amountOut, reserveIn, reserveOut);

        // Assert
        input.Should().BeGreaterThan(amountOut); // Input should be greater than output due to fees
    }

    [Fact]
    public void CalculatePriceImpact_WithValidInputs_ShouldReturnPercentage()
    {
        // Arrange
        var amm = new ConstantProductAMM(feeRate: 0.003m);
        var amountIn = 1000m;
        var reserveIn = 1000000m;
        var reserveOut = 1000000m;

        // Act
        var priceImpact = amm.CalculatePriceImpact(amountIn, reserveIn, reserveOut);

        // Assert
        priceImpact.Should().BeGreaterThanOrEqualTo(0);
        priceImpact.Should().BeLessThanOrEqualTo(1); // Should be between 0 and 100%
    }
}


























