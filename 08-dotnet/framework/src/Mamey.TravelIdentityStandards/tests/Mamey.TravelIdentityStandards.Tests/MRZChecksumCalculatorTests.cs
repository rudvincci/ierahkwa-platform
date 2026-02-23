using System.Diagnostics;
using Mamey.TravelIdentityStandards.MachineReadableZone;

namespace Mamey.TravelIdentityStandards.Tests;

public class MRZChecksumCalculatorTests
{
    [Fact]
    public void CalculateCompositeChecksum_ValidInput_ReturnsCorrectChecksum()
    {
        // Input for checksum calculation
        string input = "1234567890USA900101M300101OPTIONAL";

        // Expected checksum
        int expectedChecksum = 3; // Manually computed

        // Debugging: Log input
        Debug.WriteLine($"Testing input: {input}");

        // Calculate checksum
        int result = MRZUtils.CalculateCheckDigit(input);

        // Debugging: Log result
        Debug.WriteLine($"Computed checksum: {result}");

        // Assert the result matches the expected value
        Assert.Equal(expectedChecksum, result);
    }

    [Fact]
    public void CalculateCompositeChecksum_InvalidCharacter_ThrowsException()
    {
        string input = "INVALID#DATA";

        Assert.Throws<ArgumentException>(() => MRZUtils.CalculateCheckDigit(input));
    }
    [Fact]
    public void CalculateCompositeChecksum_EmptyInput_ThrowsException()
    {
        string input = "";
        Assert.Throws<ArgumentException>(() => MRZUtils.CalculateCheckDigit(input));
    }

    [Fact]
    public void CalculateCompositeChecksum_NullInput_ThrowsException()
    {
        string input = null;
        Assert.Throws<ArgumentException>(() => MRZUtils.CalculateCheckDigit(input));
    }
}