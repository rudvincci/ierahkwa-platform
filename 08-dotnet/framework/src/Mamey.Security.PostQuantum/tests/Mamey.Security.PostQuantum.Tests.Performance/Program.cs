using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Mamey.Security.PostQuantum.Tests.Performance;

/// <summary>
/// Performance benchmark runner for Mamey.Security.PostQuantum.
/// 
/// Usage:
///   dotnet run -c Release                    # Run all benchmarks
///   dotnet run -c Release -- --job short     # Quick benchmark run
///   dotnet run -c Release -- --filter *Sign* # Run only signature benchmarks
///   dotnet run -c Release -- --list flat     # List available benchmarks
///
/// Results are saved to BenchmarkDotNet.Artifacts/results/
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        var config = DefaultConfig.Instance
            .WithArtifactsPath("./BenchmarkDotNet.Artifacts");

        if (args.Length == 0)
        {
            // Run all benchmarks
            BenchmarkRunner.Run<SignatureBenchmarks>(config);
            BenchmarkRunner.Run<EncryptionBenchmarks>(config);
            BenchmarkRunner.Run<KeyGenerationBenchmarks>(config);
        }
        else
        {
            // Allow command-line filtering
            BenchmarkSwitcher.FromTypes(new[]
            {
                typeof(SignatureBenchmarks),
                typeof(EncryptionBenchmarks),
                typeof(KeyGenerationBenchmarks)
            }).Run(args, config);
        }

        Console.WriteLine();
        Console.WriteLine("=== Performance Summary ===");
        Console.WriteLine();
        Console.WriteLine("Expected Performance Targets (per acceptance criteria):");
        Console.WriteLine("  - ML-DSA-65 Sign: <1ms mean");
        Console.WriteLine("  - ML-DSA-65 Verify: <1ms mean");
        Console.WriteLine("  - Hybrid Sign: <1.5ms mean (acceptable overhead)");
        Console.WriteLine("  - ML-KEM-768 Encapsulate: <1ms mean");
        Console.WriteLine("  - ML-KEM-768 Decapsulate: <1ms mean");
        Console.WriteLine();
        Console.WriteLine("Results saved to: ./BenchmarkDotNet.Artifacts/results/");
    }
}


