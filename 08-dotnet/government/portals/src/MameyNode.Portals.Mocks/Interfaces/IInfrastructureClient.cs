using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks.Interfaces;

public interface IInfrastructureClient
{
    Task<NodeInfo?> GetNodeInfoAsync();
    Task<NodeBlockInfo?> GetBlockAsync(string blockHash);
    Task<NodeAccountInfo?> GetAccountAsync(string account);
    Task<List<HistoryEntryInfo>> GetAccountHistoryAsync(string account, int limit = 50);
    Task<List<PeerInfo>> GetPeersAsync();
    Task<List<RepresentativeInfo>> GetRepresentativesAsync();
    Task<BootstrapStatusInfo?> GetBootstrapStatusAsync();
    Task<NetworkInfo?> GetNetworkInfoAsync();
    Task<LedgerStatsInfo?> GetLedgerStatsAsync();
    Task<VersionInfo?> GetVersionAsync();
    Task<TelemetryInfo?> GetTelemetryAsync();
    Task<NetworkHealthInfo?> GetNetworkHealthAsync();
    Task<List<MetricDataInfo>> GetMetricsAsync();
    Task<MetricSummaryInfo?> GetMetricSummaryAsync(string metricName, DateTime startTime, DateTime endTime);
}

