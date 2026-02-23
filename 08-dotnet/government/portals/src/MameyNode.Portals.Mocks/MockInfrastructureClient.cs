using Bogus;
using MameyNode.Portals.Mocks.Interfaces;
using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks;

public class MockInfrastructureClient : IInfrastructureClient
{
    private readonly Faker _faker = new();
    private readonly List<PeerInfo> _peers = new();
    private readonly List<RepresentativeInfo> _representatives = new();
    private readonly List<HistoryEntryInfo> _accountHistory = new();
    private readonly List<MetricDataInfo> _metrics = new();

    public MockInfrastructureClient()
    {
        InitializeMockData();
    }

    private void InitializeMockData()
    {
        var peerFaker = new Faker<PeerInfo>()
            .RuleFor(p => p.PeerId, f => $"PEER-{f.Random.AlphaNumeric(8).ToUpper()}")
            .RuleFor(p => p.Address, f => f.Internet.Ip())
            .RuleFor(p => p.State, f => f.PickRandom("Connected", "Disconnected", "Connecting"))
            .RuleFor(p => p.LastSeen, f => f.Date.Recent(1))
            .RuleFor(p => p.Port, f => f.Random.Int(7000, 8000))
            .RuleFor(p => p.StateEnum, f => f.PickRandom<PeerState>())
            .RuleFor(p => p.Type, f => f.PickRandom<PeerType>())
            .RuleFor(p => p.Version, f => $"v{f.Random.Int(1, 3)}.{f.Random.Int(0, 9)}.{f.Random.Int(0, 9)}")
            .RuleFor(p => p.ConnectedAt, f => f.Date.Past(1))
            .RuleFor(p => p.BytesSent, f => (ulong)f.Random.Long(1000000, 1000000000))
            .RuleFor(p => p.BytesReceived, f => (ulong)f.Random.Long(1000000, 1000000000))
            .RuleFor(p => p.LatencyMs, f => f.Random.Int(10, 500))
            .RuleFor(p => p.VotingWeight, f => f.Random.Long(1000000, 100000000).ToString())
            .RuleFor(p => p.IsOnline, f => f.Random.Bool(0.9f));

        _peers.AddRange(peerFaker.Generate(50));

        var repFaker = new Faker<RepresentativeInfo>()
            .RuleFor(r => r.Account, f => $"0x{f.Random.AlphaNumeric(40)}")
            .RuleFor(r => r.Weight, f => f.Random.Long(1000000, 1000000000).ToString())
            .RuleFor(r => r.DelegatorsCount, f => f.Random.Int(10, 1000))
            .RuleFor(r => r.IsOnline, f => f.Random.Bool(0.85f));

        _representatives.AddRange(repFaker.Generate(20));

        var historyFaker = new Faker<HistoryEntryInfo>()
            .RuleFor(h => h.Type, f => f.PickRandom("send", "receive", "open", "change"))
            .RuleFor(h => h.Account, f => $"0x{f.Random.AlphaNumeric(40)}")
            .RuleFor(h => h.Amount, f => f.Finance.Amount(1, 100000, 2).ToString("F2"))
            .RuleFor(h => h.LocalTimestamp, f => f.Date.Recent(30))
            .RuleFor(h => h.Height, f => (ulong)f.Random.Long(1000000, 5000000))
            .RuleFor(h => h.Hash, f => $"0x{f.Random.AlphaNumeric(64)}");

        _accountHistory.AddRange(historyFaker.Generate(200));

        var metricFaker = new Faker<MetricDataInfo>()
            .RuleFor(m => m.MetricName, f => f.PickRandom("block_count", "account_count", "transaction_rate", "peer_count", "latency_ms"))
            .RuleFor(m => m.Value, f => f.Random.Double(0, 1000000))
            .RuleFor(m => m.Labels, f => new Dictionary<string, string> { { "node", $"node-{f.Random.Int(1, 10)}" } })
            .RuleFor(m => m.Timestamp, f => f.Date.Recent(1))
            .RuleFor(m => m.MetricType, f => f.PickRandom("counter", "gauge", "histogram"));

        _metrics.AddRange(metricFaker.Generate(100));
    }

    public Task<NodeInfo?> GetNodeInfoAsync()
    {
        return Task.FromResult<NodeInfo?>(new NodeInfo
        {
            Version = "1.0.0",
            NodeId = $"NODE-{_faker.Random.AlphaNumeric(8).ToUpper()}",
            BlockCount = (ulong)_faker.Random.Long(1000000, 5000000),
            AccountCount = (ulong)_faker.Random.Long(100000, 1000000),
            PeerCount = _peers.Count(p => p.IsOnline),
            ConfirmationHeight = (ulong)_faker.Random.Long(1000000, 5000000)
        });
    }

    public Task<NodeBlockInfo?> GetBlockAsync(string blockHash)
    {
        return Task.FromResult<NodeBlockInfo?>(new NodeBlockInfo
        {
            BlockHash = blockHash,
            Previous = $"0x{_faker.Random.AlphaNumeric(64)}",
            Account = $"0x{_faker.Random.AlphaNumeric(40)}",
            Timestamp = _faker.Date.Recent(7),
            BlockData = _faker.Random.Bytes(128)
        });
    }

    public Task<NodeAccountInfo?> GetAccountAsync(string account)
    {
        return Task.FromResult<NodeAccountInfo?>(new NodeAccountInfo
        {
            Account = account,
            HeadBlock = $"0x{_faker.Random.AlphaNumeric(64)}",
            Representative = $"0x{_faker.Random.AlphaNumeric(40)}",
            Balance = _faker.Finance.Amount(1000, 1000000, 2).ToString("F2"),
            BlockCount = (ulong)_faker.Random.Long(1, 10000),
            Exists = true
        });
    }

    public Task<List<HistoryEntryInfo>> GetAccountHistoryAsync(string account, int limit = 50) => Task.FromResult(_accountHistory.Take(limit).ToList());
    public Task<List<PeerInfo>> GetPeersAsync() => Task.FromResult(_peers);
    public Task<List<RepresentativeInfo>> GetRepresentativesAsync() => Task.FromResult(_representatives);
    public Task<BootstrapStatusInfo?> GetBootstrapStatusAsync()
    {
        return Task.FromResult<BootstrapStatusInfo?>(new BootstrapStatusInfo
        {
            IsBootstrapping = _faker.Random.Bool(0.2f),
            BootstrapTarget = (ulong)_faker.Random.Long(5000000, 10000000),
            CurrentBlock = (ulong)_faker.Random.Long(4000000, 5000000),
            Connections = _faker.Random.Int(5, 20),
            Phase = _faker.PickRandom("Idle", "Pulling", "Processing"),
            RemainingBlocks = (ulong)_faker.Random.Long(0, 1000000)
        });
    }

    public Task<NetworkInfo?> GetNetworkInfoAsync()
    {
        return Task.FromResult<NetworkInfo?>(new NetworkInfo
        {
            ActivePeers = _peers.Count(p => p.IsOnline),
            MaxPeers = 100,
            BytesSent = (ulong)_peers.Sum(p => (long)p.BytesSent),
            BytesReceived = (ulong)_peers.Sum(p => (long)p.BytesReceived),
            ProtocolVersion = "1.0",
            ActiveTransactions = _faker.Random.Int(0, 1000),
            AverageLatencyMs = _peers.Any() ? _peers.Average(p => p.LatencyMs) : 0
        });
    }

    public Task<LedgerStatsInfo?> GetLedgerStatsAsync()
    {
        return Task.FromResult<LedgerStatsInfo?>(new LedgerStatsInfo
        {
            AccountCount = (ulong)_faker.Random.Long(100000, 1000000),
            BlockCount = (ulong)_faker.Random.Long(1000000, 5000000),
            CementedCount = (ulong)_faker.Random.Long(900000, 5000000),
            UncheckedCount = (ulong)_faker.Random.Long(0, 100000),
            DatabaseSize = $"{_faker.Random.Int(10, 100)}GB",
            PruningTarget = (ulong)_faker.Random.Long(1000000, 2000000)
        });
    }

    public Task<VersionInfo?> GetVersionAsync()
    {
        return Task.FromResult<VersionInfo?>(new VersionInfo
        {
            RpcVersion = 1,
            StoreVersion = 1,
            ProtocolVersion = 1,
            NodeVendor = "Mamey Technologies",
            StoreVendor = "Mamey Technologies",
            Network = "mainnet",
            NetworkIdentifier = _faker.Random.AlphaNumeric(64),
            BuildInfo = "Release"
        });
    }

    public Task<TelemetryInfo?> GetTelemetryAsync()
    {
        return Task.FromResult<TelemetryInfo?>(new TelemetryInfo
        {
            BlockCount = (ulong)_faker.Random.Long(1000000, 5000000),
            CementedCount = (ulong)_faker.Random.Long(900000, 5000000),
            UncheckedCount = (ulong)_faker.Random.Long(0, 100000),
            AccountCount = (ulong)_faker.Random.Long(100000, 1000000),
            BandwidthCap = (ulong)_faker.Random.Long(100000000, 1000000000),
            PeerCount = _peers.Count,
            ProtocolVersion = 1,
            Uptime = (ulong)_faker.Random.Long(86400, 2592000),
            GenesisBlock = $"0x{_faker.Random.AlphaNumeric(64)}",
            MajorVersion = "1",
            MinorVersion = "0",
            PatchVersion = "0",
            PreReleaseVersion = "",
            Maker = "Mamey Technologies",
            Timestamp = DateTime.Now
        });
    }

    public Task<NetworkHealthInfo?> GetNetworkHealthAsync()
    {
        return Task.FromResult<NetworkHealthInfo?>(new NetworkHealthInfo
        {
            IsHealthy = _faker.Random.Bool(0.9f),
            HealthScore = _faker.Random.Float(0.7f, 1.0f),
            ConnectedPeers = _peers.Count(p => p.IsOnline),
            MinRequiredPeers = 10,
            HasQuorum = _faker.Random.Bool(0.95f),
            OnlineWeightPercentage = _faker.Random.Float(0.8f, 1.0f),
            ActiveRepresentatives = _representatives.Count(r => r.IsOnline),
            IsSynced = _faker.Random.Bool(0.9f),
            SyncStatus = _faker.PickRandom("Synced", "Syncing", "Behind"),
            Warnings = _faker.Random.Bool(0.1f) ? _faker.Lorem.Words(_faker.Random.Int(1, 2)).ToList() : new List<string>()
        });
    }

    public Task<List<MetricDataInfo>> GetMetricsAsync() => Task.FromResult(_metrics);
    public Task<MetricSummaryInfo?> GetMetricSummaryAsync(string metricName, DateTime startTime, DateTime endTime)
    {
        var relevantMetrics = _metrics.Where(m => m.MetricName == metricName && m.Timestamp >= startTime && m.Timestamp <= endTime).ToList();
        if (!relevantMetrics.Any()) return Task.FromResult<MetricSummaryInfo?>(null);

        var values = relevantMetrics.Select(m => m.Value).ToList();
        return Task.FromResult<MetricSummaryInfo?>(new MetricSummaryInfo
        {
            MetricName = metricName,
            Min = values.Min(),
            Max = values.Max(),
            Average = values.Average(),
            Sum = values.Sum(),
            Count = (ulong)values.Count,
            P50 = values.OrderBy(v => v).Skip(values.Count / 2).First(),
            P95 = values.OrderBy(v => v).Skip((int)(values.Count * 0.95)).FirstOrDefault(),
            P99 = values.OrderBy(v => v).Skip((int)(values.Count * 0.99)).FirstOrDefault()
        });
    }
}

