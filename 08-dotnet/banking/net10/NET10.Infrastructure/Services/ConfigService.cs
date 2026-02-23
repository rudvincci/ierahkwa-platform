using NET10.Core.Interfaces;
using NET10.Core.Models;

namespace NET10.Infrastructure.Services;

public class ConfigService : IConfigService
{
    private NET10Config _config;
    private readonly List<DeployedContract> _contracts;
    private readonly IPoolService _poolService;
    private readonly IFarmService _farmService;

    public ConfigService(IPoolService poolService, IFarmService farmService)
    {
        _poolService = poolService;
        _farmService = farmService;
        
        _config = new NET10Config
        {
            PlatformName = "Ierahkwa NET10 DeFi",
            LogoUrl = "/assets/logo.png",
            Theme = "dark",
            PrimaryColor = "#FFD700",
            SecondaryColor = "#00FF41",
            DefaultChainId = "777777",
            SupportedChains = new[] { "777777", "1", "56", "137", "43114" },
            NodeEndpoint = "https://node.ierahkwa.gov",
            DefaultSwapFee = 0.003m,
            AdminFeePercent = 0.1m,
            LPFeePercent = 0.2m,
            FeeRecipient = "0xIerahkwa...AdminTreasury",
            DefaultSlippage = 0.5m,
            MaxSlippage = 50m,
            MinTradeAmount = 0.0001m,
            MaxTradeAmount = 1000000m,
            TransactionDeadlineMinutes = 20,
            MinLiquidityAmount = 100m,
            SwapEnabled = true,
            LiquidityEnabled = true,
            FarmingEnabled = true,
            ChartsEnabled = true,
            AnalyticsEnabled = true,
            RouterAddress = "0xIerahkwa...Router",
            FactoryAddress = "0xIerahkwa...Factory",
            MasterChefAddress = "0xIerahkwa...MasterChef",
            FiatOnRampEnabled = false
        };

        _contracts = new List<DeployedContract>
        {
            new DeployedContract
            {
                Id = "contract-router",
                ContractType = "Router",
                Name = "IerahkwaRouter",
                Address = "0xIerahkwa...Router",
                ChainId = "777777",
                IsVerified = true,
                DeployedAt = DateTime.UtcNow.AddDays(-90)
            },
            new DeployedContract
            {
                Id = "contract-factory",
                ContractType = "Factory",
                Name = "IerahkwaFactory",
                Address = "0xIerahkwa...Factory",
                ChainId = "777777",
                IsVerified = true,
                DeployedAt = DateTime.UtcNow.AddDays(-90)
            },
            new DeployedContract
            {
                Id = "contract-masterchef",
                ContractType = "MasterChef",
                Name = "IerahkwaMasterChef",
                Address = "0xIerahkwa...MasterChef",
                ChainId = "777777",
                IsVerified = true,
                DeployedAt = DateTime.UtcNow.AddDays(-60)
            }
        };
    }

    public Task<NET10Config> GetConfigAsync()
    {
        return Task.FromResult(_config);
    }

    public Task<NET10Config> UpdateConfigAsync(NET10Config config)
    {
        _config = config;
        return Task.FromResult(_config);
    }

    public async Task<AdminStats> GetStatsAsync()
    {
        var pools = await _poolService.GetAllPoolsAsync();
        var farms = await _farmService.GetAllFarmsAsync();

        return new AdminStats
        {
            TotalVolume = 125000000,
            Volume24h = pools.Sum(p => p.Volume24h),
            Volume7d = pools.Sum(p => p.Volume24h) * 7,
            Volume30d = pools.Sum(p => p.Volume24h) * 30,
            TotalTVL = pools.Sum(p => p.TVL) + farms.Sum(f => f.TVL),
            PoolsTVL = pools.Sum(p => p.TVL),
            FarmsTVL = farms.Sum(f => f.TVL),
            TotalFees = 250000,
            AdminFees = 25000,
            LPFees = 225000,
            Fees24h = pools.Sum(p => p.Fees24h),
            TotalSwaps = 125000,
            Swaps24h = 2500,
            TotalUsers = 15000,
            ActiveUsers24h = 1200,
            TotalPools = pools.Count,
            TotalFarms = farms.Count,
            TotalTokens = 14,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public Task<List<DeployedContract>> GetDeployedContractsAsync()
    {
        return Task.FromResult(_contracts.ToList());
    }

    public Task<DeployedContract> DeployContractAsync(ContractDeployRequest request)
    {
        var contract = new DeployedContract
        {
            Id = Guid.NewGuid().ToString(),
            ContractType = request.ContractType,
            Name = request.Name,
            Address = $"0x{Guid.NewGuid():N}".Substring(0, 42),
            TxHash = $"0x{Guid.NewGuid():N}",
            ChainId = request.ChainId,
            IsVerified = false,
            DeployedAt = DateTime.UtcNow
        };

        _contracts.Add(contract);
        return Task.FromResult(contract);
    }
}
