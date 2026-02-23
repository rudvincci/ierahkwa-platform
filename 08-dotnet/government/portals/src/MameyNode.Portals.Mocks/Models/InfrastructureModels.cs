namespace MameyNode.Portals.Mocks.Models;

// Models based on node.proto, rpc.proto, network.proto, and metrics.proto

// Node Models (from node.proto)
public class NodeInfo
{
    public string Version { get; set; } = string.Empty;
    public string NodeId { get; set; } = string.Empty;
    public ulong BlockCount { get; set; }
    public ulong AccountCount { get; set; }
    public int PeerCount { get; set; }
    public ulong ConfirmationHeight { get; set; }
}

public class NodeBlockInfo
{
    public string BlockHash { get; set; } = string.Empty;
    public string Previous { get; set; } = string.Empty;
    public string Account { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public byte[] BlockData { get; set; } = Array.Empty<byte>();
}

public class NodeAccountInfo
{
    public string Account { get; set; } = string.Empty;
    public string HeadBlock { get; set; } = string.Empty;
    public string Representative { get; set; } = string.Empty;
    public string Balance { get; set; } = "0";
    public ulong BlockCount { get; set; }
    public bool Exists { get; set; }
}

public class PeerInfo
{
    public string PeerId { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string State { get; set; } = "Connected";
    public DateTime LastSeen { get; set; }
    public int Port { get; set; }
    public PeerState StateEnum { get; set; }
    public PeerType Type { get; set; }
    public string Version { get; set; } = string.Empty;
    public DateTime ConnectedAt { get; set; }
    public ulong BytesSent { get; set; }
    public ulong BytesReceived { get; set; }
    public int LatencyMs { get; set; }
    public string VotingWeight { get; set; } = "0";
    public bool IsOnline { get; set; }
}

public enum PeerState
{
    Unknown = 0,
    Disconnected = 1,
    Connecting = 2,
    Connected = 3,
    Handshaking = 4,
    Syncing = 5,
    Ready = 6,
    Disconnecting = 7
}

public enum PeerType
{
    Unknown = 0,
    Regular = 1,
    Bootstrap = 2,
    Validator = 3,
    Representative = 4
}

public class RepresentativeInfo
{
    public string Account { get; set; } = string.Empty;
    public string Weight { get; set; } = "0";
    public int DelegatorsCount { get; set; }
    public bool IsOnline { get; set; }
}

public class BootstrapStatusInfo
{
    public bool IsBootstrapping { get; set; }
    public ulong BootstrapTarget { get; set; }
    public ulong CurrentBlock { get; set; }
    public int Connections { get; set; }
    public string Phase { get; set; } = "Idle";
    public ulong RemainingBlocks { get; set; }
}

public class NetworkInfo
{
    public int ActivePeers { get; set; }
    public int MaxPeers { get; set; }
    public ulong BytesSent { get; set; }
    public ulong BytesReceived { get; set; }
    public string ProtocolVersion { get; set; } = string.Empty;
    public int ActiveTransactions { get; set; }
    public double AverageLatencyMs { get; set; }
}

public class LedgerStatsInfo
{
    public ulong AccountCount { get; set; }
    public ulong BlockCount { get; set; }
    public ulong CementedCount { get; set; }
    public ulong UncheckedCount { get; set; }
    public string DatabaseSize { get; set; } = "0";
    public ulong PruningTarget { get; set; }
}

// RPC Models (from rpc.proto)
public class HistoryEntryInfo
{
    public string Type { get; set; } = string.Empty;
    public string Account { get; set; } = string.Empty;
    public string Amount { get; set; } = "0";
    public DateTime LocalTimestamp { get; set; }
    public ulong Height { get; set; }
    public string Hash { get; set; } = string.Empty;
}

public class PendingBlockInfo
{
    public string Hash { get; set; } = string.Empty;
    public string Amount { get; set; } = "0";
    public string Source { get; set; } = string.Empty;
}

public class BalanceInfo
{
    public string Balance { get; set; } = "0";
    public string Pending { get; set; } = "0";
}

public class VersionInfo
{
    public int RpcVersion { get; set; }
    public int StoreVersion { get; set; }
    public int ProtocolVersion { get; set; }
    public string NodeVendor { get; set; } = string.Empty;
    public string StoreVendor { get; set; } = string.Empty;
    public string Network { get; set; } = string.Empty;
    public string NetworkIdentifier { get; set; } = string.Empty;
    public string BuildInfo { get; set; } = string.Empty;
}

public class TelemetryInfo
{
    public ulong BlockCount { get; set; }
    public ulong CementedCount { get; set; }
    public ulong UncheckedCount { get; set; }
    public ulong AccountCount { get; set; }
    public ulong BandwidthCap { get; set; }
    public int PeerCount { get; set; }
    public int ProtocolVersion { get; set; }
    public ulong Uptime { get; set; }
    public string GenesisBlock { get; set; } = string.Empty;
    public string MajorVersion { get; set; } = string.Empty;
    public string MinorVersion { get; set; } = string.Empty;
    public string PatchVersion { get; set; } = string.Empty;
    public string PreReleaseVersion { get; set; } = string.Empty;
    public string Maker { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

// Network Models (from network.proto)
public class BootstrapStatus
{
    public string BootstrapId { get; set; } = string.Empty;
    public BootstrapStrategy Strategy { get; set; }
    public bool IsActive { get; set; }
    public ulong BlocksDownloaded { get; set; }
    public ulong BlocksProcessed { get; set; }
    public ulong TotalBlocks { get; set; }
    public float ProgressPercentage { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EstimatedCompletion { get; set; }
    public string CurrentAccount { get; set; } = string.Empty;
}

public enum BootstrapStrategy
{
    Unknown = 0,
    Legacy = 1,
    Lazy = 2,
    WalletLazy = 3
}

public class BootstrapPeerInfo
{
    public string Address { get; set; } = string.Empty;
    public int Port { get; set; }
    public int Priority { get; set; }
    public bool IsOnline { get; set; }
    public DateTime LastSuccess { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
}

public class NetworkStatsInfo
{
    public int TotalPeers { get; set; }
    public int ConnectedPeers { get; set; }
    public int SyncingPeers { get; set; }
    public int RepresentativePeers { get; set; }
    public ulong TotalBytesSent { get; set; }
    public ulong TotalBytesReceived { get; set; }
    public ulong MessagesSent1m { get; set; }
    public ulong MessagesReceived1m { get; set; }
    public float AvgLatencyMs { get; set; }
    public string TotalVotingWeight { get; set; } = "0";
    public string OnlineVotingWeight { get; set; } = "0";
}

public class BandwidthStatsInfo
{
    public ulong BytesSent { get; set; }
    public ulong BytesReceived { get; set; }
    public ulong PacketsSent { get; set; }
    public ulong PacketsReceived { get; set; }
    public float AvgUploadRate { get; set; }
    public float AvgDownloadRate { get; set; }
}

public class MessageTypeStatInfo
{
    public MessageType Type { get; set; }
    public ulong SentCount { get; set; }
    public ulong ReceivedCount { get; set; }
    public ulong DroppedCount { get; set; }
    public ulong TotalBytes { get; set; }
}

public enum MessageType
{
    Unknown = 0,
    Keepalive = 1,
    Publish = 2,
    ConfirmReq = 3,
    ConfirmAck = 4,
    BulkPull = 5,
    BulkPush = 6,
    FrontierReq = 7,
    NodeIdHandshake = 8,
    TelemetryReq = 9,
    TelemetryAck = 10
}

public class NetworkHealthInfo
{
    public bool IsHealthy { get; set; }
    public float HealthScore { get; set; }
    public int ConnectedPeers { get; set; }
    public int MinRequiredPeers { get; set; }
    public bool HasQuorum { get; set; }
    public float OnlineWeightPercentage { get; set; }
    public int ActiveRepresentatives { get; set; }
    public bool IsSynced { get; set; }
    public string SyncStatus { get; set; } = string.Empty;
    public List<string> Warnings { get; set; } = new();
}

public class RateLimitInfo
{
    public string PeerId { get; set; } = string.Empty;
    public MessageType MessageType { get; set; }
    public ulong RequestsAllowed { get; set; }
    public ulong RequestsBlocked { get; set; }
    public ulong CurrentRate { get; set; }
    public ulong LimitRate { get; set; }
    public bool IsThrottled { get; set; }
}

// Metrics Models (from metrics.proto)
public class MetricDataInfo
{
    public string MetricName { get; set; } = string.Empty;
    public double Value { get; set; }
    public Dictionary<string, string> Labels { get; set; } = new();
    public DateTime Timestamp { get; set; }
    public string MetricType { get; set; } = string.Empty;
}

public class CounterDataInfo
{
    public string CounterName { get; set; } = string.Empty;
    public double Value { get; set; }
    public Dictionary<string, string> Labels { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}

public class HistogramDataInfo
{
    public string HistogramName { get; set; } = string.Empty;
    public List<HistogramBucketInfo> Buckets { get; set; } = new();
    public double Sum { get; set; }
    public ulong Count { get; set; }
    public Dictionary<string, string> Labels { get; set; } = new();
}

public class HistogramBucketInfo
{
    public double UpperBound { get; set; }
    public ulong Count { get; set; }
}

public class GaugeDataInfo
{
    public string GaugeName { get; set; } = string.Empty;
    public double Value { get; set; }
    public Dictionary<string, string> Labels { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}

public class MetricSummaryInfo
{
    public string MetricName { get; set; } = string.Empty;
    public double Min { get; set; }
    public double Max { get; set; }
    public double Average { get; set; }
    public double Sum { get; set; }
    public ulong Count { get; set; }
    public double P50 { get; set; }
    public double P95 { get; set; }
    public double P99 { get; set; }
}

