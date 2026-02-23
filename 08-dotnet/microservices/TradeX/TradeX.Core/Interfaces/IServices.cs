using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradeX.Core.Models;

namespace TradeX.Core.Interfaces;

/// <summary>
/// Ierahkwa Node Service Interface
/// Direct integration with Ierahkwa Futurehead Mamey Node
/// </summary>
public interface IIerahkwaNodeService
{
    Task<bool> IsConnectedAsync();
    Task<decimal> GetBalanceAsync(string address, string assetSymbol);
    Task<string> TransferAsync(string fromAddress, string toAddress, decimal amount, string assetSymbol);
    Task<string> GetTransactionStatusAsync(string txHash);
    Task<decimal> GetAssetPriceAsync(string assetSymbol);
    Task<string> GenerateWalletAsync();
    Task<bool> ValidateAddressAsync(string address);
}

/// <summary>
/// Trading Service Interface
/// </summary>
public interface ITradingService
{
    Task<Order> PlaceOrderAsync(Guid userId, Guid tradingPairId, OrderSide side, OrderType type, decimal amount, decimal price, decimal? stopPrice = null);
    Task<Order?> CancelOrderAsync(Guid orderId, Guid userId);
    Task<IEnumerable<Order>> GetOpenOrdersAsync(Guid userId);
    Task<IEnumerable<Order>> GetOrderHistoryAsync(Guid userId, int page = 1, int pageSize = 50);
    Task<IEnumerable<Trade>> GetTradeHistoryAsync(Guid userId, int page = 1, int pageSize = 50);
    Task<IEnumerable<TradingPair>> GetTradingPairsAsync();
    Task<TradingPair?> GetTradingPairAsync(Guid id);
    Task MatchOrdersAsync(Guid tradingPairId);
}

/// <summary>
/// Wallet Service Interface
/// </summary>
public interface IWalletService
{
    Task<Wallet> GetOrCreateWalletAsync(Guid userId, Guid assetId);
    Task<IEnumerable<Wallet>> GetUserWalletsAsync(Guid userId);
    Task<decimal> GetBalanceAsync(Guid userId, Guid assetId);
    Task<Transaction> DepositAsync(Guid userId, Guid assetId, decimal amount, string txHash);
    Task<Transaction> WithdrawAsync(Guid userId, Guid assetId, decimal amount, string toAddress);
    Task<Transaction> TransferAsync(Guid fromUserId, Guid toUserId, Guid assetId, decimal amount);
    Task<IEnumerable<Transaction>> GetTransactionHistoryAsync(Guid userId, int page = 1, int pageSize = 50, bool vipOnly = false);
    /// <summary>Transacciones VIP (prioridad) — opcionalmente filtradas por userId.</summary>
    Task<IEnumerable<Transaction>> GetVipTransactionsAsync(Guid? userId, int page = 1, int pageSize = 50);
    Task SetUserVipLevelAsync(Guid userId, VipLevel level);
}

/// <summary>
/// Swap Service Interface
/// </summary>
public interface ISwapService
{
    Task<decimal> GetExchangeRateAsync(Guid fromAssetId, Guid toAssetId);
    Task<SwapRequest> SwapAsync(Guid userId, Guid fromAssetId, Guid toAssetId, decimal amount);
    Task<IEnumerable<SwapRequest>> GetSwapHistoryAsync(Guid userId);
}

/// <summary>
/// Staking Service Interface
/// </summary>
public interface IStakingService
{
    Task<IEnumerable<StakingPool>> GetActivePoolsAsync();
    Task<StakingPool?> GetPoolAsync(Guid poolId);
    Task<Stake> StakeAsync(Guid userId, Guid poolId, decimal amount);
    Task<Stake> UnstakeAsync(Guid stakeId, Guid userId, bool early = false);
    Task<decimal> ClaimRewardsAsync(Guid stakeId, Guid userId);
    Task<IEnumerable<Stake>> GetUserStakesAsync(Guid userId);
    Task CalculateRewardsAsync();
}

/// <summary>
/// P2P Service Interface
/// </summary>
public interface IP2PService
{
    Task<P2PAd> CreateAdAsync(Guid userId, P2PAd ad);
    Task<IEnumerable<P2PAd>> GetAdsAsync(P2PAdType? type = null, Guid? assetId = null);
    Task<P2POrder> CreateOrderAsync(Guid buyerId, Guid adId, decimal amount);
    Task<P2POrder> LockEscrowAsync(Guid orderId);
    Task<P2POrder> MarkAsPaidAsync(Guid orderId, Guid buyerId);
    Task<P2POrder> ReleaseCryptoAsync(Guid orderId, Guid sellerId);
    Task<P2POrder> CancelOrderAsync(Guid orderId, Guid userId);
    Task<P2POrder> OpenDisputeAsync(Guid orderId, Guid userId, string reason);
}

/// <summary>
/// KYC Service Interface
/// </summary>
public interface IKycService
{
    Task<bool> SubmitKycAsync(Guid userId, KycDocument document);
    Task<KycStatus> GetKycStatusAsync(Guid userId);
    Task<bool> ApproveKycAsync(Guid userId, Guid adminId);
    Task<bool> RejectKycAsync(Guid userId, Guid adminId, string reason);
}

/// <summary>
/// KYC Document Model
/// </summary>
public class KycDocument
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    
    public string DocumentType { get; set; } = string.Empty; // Passport, ID, License
    public string DocumentNumber { get; set; } = string.Empty;
    public string FrontImageUrl { get; set; } = string.Empty;
    public string? BackImageUrl { get; set; }
    public string? SelfieUrl { get; set; }
    
    public string Country { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public DateTime? ExpiryDate { get; set; }
    
    public KycStatus Status { get; set; } = KycStatus.Pending;
    public string? RejectionReason { get; set; }
    
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }
    public Guid? ReviewedBy { get; set; }
}
