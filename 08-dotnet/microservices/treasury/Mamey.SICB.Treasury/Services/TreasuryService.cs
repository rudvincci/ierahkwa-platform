using Mamey.SICB.Treasury.Models;

namespace Mamey.SICB.Treasury.Services;

public interface ITreasuryService
{
    // Operations
    Task<TreasuryOperation> CreateOperationAsync(CreateOperationRequest request);
    Task<TreasuryOperation?> GetOperationAsync(Guid id);
    Task<IReadOnlyList<TreasuryOperation>> GetOperationsAsync(OperationFilter? filter = null);
    Task<bool> ApproveOperationAsync(Guid id, ApprovalRequest request);
    Task<bool> ExecuteOperationAsync(Guid id);
    Task<bool> CancelOperationAsync(Guid id, string reason);
    
    // Issuance
    Task<CurrencyIssuance> CreateIssuanceAsync(CreateIssuanceRequest request);
    Task<CurrencyIssuance?> GetIssuanceAsync(Guid id);
    Task<bool> ExecuteIssuanceAsync(Guid id);
    
    // Accounts
    Task<TreasuryAccount> CreateAccountAsync(CreateAccountRequest request);
    Task<TreasuryAccount?> GetAccountAsync(Guid id);
    Task<IReadOnlyList<TreasuryAccount>> GetAccountsAsync();
    Task<Dictionary<string, decimal>> GetBalancesAsync(Guid accountId);
    
    // Reports
    Task<TreasuryReport> GetDailyReportAsync(DateTime date);
    Task<TreasuryReport> GetMonthlyReportAsync(int year, int month);
}

public class TreasuryService : ITreasuryService
{
    private readonly Dictionary<Guid, TreasuryOperation> _operations = new();
    private readonly Dictionary<Guid, CurrencyIssuance> _issuances = new();
    private readonly Dictionary<Guid, TreasuryAccount> _accounts = new();
    private long _operationCounter = 1000000;
    
    public TreasuryService()
    {
        // Initialize default accounts
        InitializeDefaultAccounts();
    }
    
    private void InitializeDefaultAccounts()
    {
        var reserveAccount = new TreasuryAccount
        {
            AccountId = "SICB-RESERVE-001",
            Name = "Main Reserve Account",
            Type = TreasuryAccountType.Reserve,
            Balances = new Dictionary<string, decimal>
            {
                ["WAMPUM"] = 1_000_000_000,
                ["SICBDC"] = 500_000_000,
                ["USD"] = 100_000_000
            },
            DailyLimit = 10_000_000,
            MonthlyLimit = 100_000_000,
            RequiredSignatures = 3
        };
        _accounts[reserveAccount.Id] = reserveAccount;
        
        var operatingAccount = new TreasuryAccount
        {
            AccountId = "SICB-OPERATING-001",
            Name = "Operating Account",
            Type = TreasuryAccountType.Operating,
            Balances = new Dictionary<string, decimal>
            {
                ["WAMPUM"] = 10_000_000,
                ["USD"] = 1_000_000
            },
            DailyLimit = 1_000_000,
            MonthlyLimit = 10_000_000,
            RequiredSignatures = 2
        };
        _accounts[operatingAccount.Id] = operatingAccount;
        
        var humanitarianAccount = new TreasuryAccount
        {
            AccountId = "SICB-HUMANITARIAN-001",
            Name = "Humanitarian Fund (10%)",
            Type = TreasuryAccountType.Humanitarian,
            Balances = new Dictionary<string, decimal>
            {
                ["WAMPUM"] = 100_000_000
            },
            DailyLimit = 500_000,
            MonthlyLimit = 5_000_000,
            RequiredSignatures = 2
        };
        _accounts[humanitarianAccount.Id] = humanitarianAccount;
    }
    
    public async Task<TreasuryOperation> CreateOperationAsync(CreateOperationRequest request)
    {
        var opId = Interlocked.Increment(ref _operationCounter);
        
        var operation = new TreasuryOperation
        {
            OperationId = $"SICB-OP-{opId}",
            Type = request.Type,
            Amount = request.Amount,
            Currency = request.Currency,
            FromAccount = request.FromAccount,
            ToAccount = request.ToAccount,
            BeneficiaryId = request.BeneficiaryId,
            BeneficiaryName = request.BeneficiaryName,
            InitiatedBy = request.InitiatedBy,
            Description = request.Description,
            Category = request.Category,
            TreatyReference = request.TreatyReference,
            RequiresTreatyCompliance = request.RequiresTreatyCompliance,
            RequiresGovernanceVote = request.Amount > 1_000_000, // Auto-require for large amounts
            RequiredApprovals = request.Amount > 100_000 ? 2 : 1
        };
        
        operation.Status = TreasuryOperationStatus.AwaitingApproval;
        _operations[operation.Id] = operation;
        
        return await Task.FromResult(operation);
    }
    
    public Task<TreasuryOperation?> GetOperationAsync(Guid id)
    {
        _operations.TryGetValue(id, out var operation);
        return Task.FromResult(operation);
    }
    
    public Task<IReadOnlyList<TreasuryOperation>> GetOperationsAsync(OperationFilter? filter = null)
    {
        IEnumerable<TreasuryOperation> query = _operations.Values;
        
        if (filter != null)
        {
            if (filter.Type.HasValue)
                query = query.Where(o => o.Type == filter.Type.Value);
            if (filter.Status.HasValue)
                query = query.Where(o => o.Status == filter.Status.Value);
            if (filter.FromDate.HasValue)
                query = query.Where(o => o.CreatedAt >= filter.FromDate.Value);
            if (filter.ToDate.HasValue)
                query = query.Where(o => o.CreatedAt <= filter.ToDate.Value);
        }
        
        return Task.FromResult<IReadOnlyList<TreasuryOperation>>(query.OrderByDescending(o => o.CreatedAt).ToList());
    }
    
    public async Task<bool> ApproveOperationAsync(Guid id, ApprovalRequest request)
    {
        if (!_operations.TryGetValue(id, out var operation))
            return false;
        
        var approval = new TreasuryApproval
        {
            OperationId = id,
            ApproverId = request.ApproverId,
            ApproverName = request.ApproverName,
            ApproverRole = request.ApproverRole,
            Decision = request.Decision,
            Comments = request.Comments,
            Signature = request.Signature
        };
        
        operation.Approvals.Add(approval);
        
        if (request.Decision == ApprovalDecision.Approved)
        {
            operation.CurrentApprovals++;
            
            if (operation.CurrentApprovals >= operation.RequiredApprovals)
            {
                operation.Status = TreasuryOperationStatus.Approved;
                operation.ApprovedBy = request.ApproverId;
                operation.ApprovedAt = DateTime.UtcNow;
            }
        }
        else if (request.Decision == ApprovalDecision.Rejected)
        {
            operation.Status = TreasuryOperationStatus.Rejected;
        }
        
        return await Task.FromResult(true);
    }
    
    public async Task<bool> ExecuteOperationAsync(Guid id)
    {
        if (!_operations.TryGetValue(id, out var operation))
            return false;
        
        if (operation.Status != TreasuryOperationStatus.Approved)
            return false;
        
        operation.Status = TreasuryOperationStatus.Processing;
        
        // Simulate blockchain transaction
        operation.TransactionHash = $"0x{Guid.NewGuid():N}";
        operation.BlockNumber = (ulong)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 3);
        
        operation.Status = TreasuryOperationStatus.Executed;
        operation.ExecutedAt = DateTime.UtcNow;
        operation.CompletedAt = DateTime.UtcNow;
        operation.Status = TreasuryOperationStatus.Completed;
        
        return await Task.FromResult(true);
    }
    
    public async Task<bool> CancelOperationAsync(Guid id, string reason)
    {
        if (!_operations.TryGetValue(id, out var operation))
            return false;
        
        if (operation.Status == TreasuryOperationStatus.Completed || 
            operation.Status == TreasuryOperationStatus.Executed)
            return false;
        
        operation.Status = TreasuryOperationStatus.Cancelled;
        operation.Metadata ??= new Dictionary<string, string>();
        operation.Metadata["cancellation_reason"] = reason;
        
        return await Task.FromResult(true);
    }
    
    public async Task<CurrencyIssuance> CreateIssuanceAsync(CreateIssuanceRequest request)
    {
        var issuance = new CurrencyIssuance
        {
            IssuanceId = $"SICB-MINT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8]}",
            Type = request.Type,
            CurrencySymbol = request.CurrencySymbol,
            Amount = request.Amount,
            RecipientAccount = request.RecipientAccount,
            BackingAsset = request.BackingAsset,
            BackingAmount = request.BackingAmount,
            AuthorizedBy = request.AuthorizedBy,
            TreatyReference = request.TreatyReference
        };
        
        _issuances[issuance.Id] = issuance;
        return await Task.FromResult(issuance);
    }
    
    public Task<CurrencyIssuance?> GetIssuanceAsync(Guid id)
    {
        _issuances.TryGetValue(id, out var issuance);
        return Task.FromResult(issuance);
    }
    
    public async Task<bool> ExecuteIssuanceAsync(Guid id)
    {
        if (!_issuances.TryGetValue(id, out var issuance))
            return false;
        
        issuance.Status = IssuanceStatus.Minted;
        issuance.MintTransactionHash = $"0x{Guid.NewGuid():N}";
        issuance.BlockNumber = (ulong)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 3);
        issuance.IssuedAt = DateTime.UtcNow;
        issuance.Status = IssuanceStatus.Completed;
        
        return await Task.FromResult(true);
    }
    
    public async Task<TreasuryAccount> CreateAccountAsync(CreateAccountRequest request)
    {
        var account = new TreasuryAccount
        {
            AccountId = $"SICB-{request.Type.ToString().ToUpper()}-{Guid.NewGuid().ToString()[..8]}",
            Name = request.Name,
            Type = request.Type,
            DailyLimit = request.DailyLimit,
            MonthlyLimit = request.MonthlyLimit,
            RequiredSignatures = request.RequiredSignatures,
            Signatories = request.Signatories
        };
        
        _accounts[account.Id] = account;
        return await Task.FromResult(account);
    }
    
    public Task<TreasuryAccount?> GetAccountAsync(Guid id)
    {
        _accounts.TryGetValue(id, out var account);
        return Task.FromResult(account);
    }
    
    public Task<IReadOnlyList<TreasuryAccount>> GetAccountsAsync()
    {
        return Task.FromResult<IReadOnlyList<TreasuryAccount>>(_accounts.Values.ToList());
    }
    
    public Task<Dictionary<string, decimal>> GetBalancesAsync(Guid accountId)
    {
        if (_accounts.TryGetValue(accountId, out var account))
            return Task.FromResult(account.Balances);
        
        return Task.FromResult(new Dictionary<string, decimal>());
    }
    
    public Task<TreasuryReport> GetDailyReportAsync(DateTime date)
    {
        var operations = _operations.Values
            .Where(o => o.CreatedAt.Date == date.Date)
            .ToList();
        
        return Task.FromResult(new TreasuryReport
        {
            ReportDate = date,
            TotalOperations = operations.Count,
            TotalDisbursements = operations.Where(o => o.Type == TreasuryOperationType.Disbursement).Sum(o => o.Amount),
            TotalIssuances = operations.Where(o => o.Type == TreasuryOperationType.Issuance).Sum(o => o.Amount),
            PendingOperations = operations.Count(o => o.Status == TreasuryOperationStatus.Pending || o.Status == TreasuryOperationStatus.AwaitingApproval),
            CompletedOperations = operations.Count(o => o.Status == TreasuryOperationStatus.Completed)
        });
    }
    
    public Task<TreasuryReport> GetMonthlyReportAsync(int year, int month)
    {
        var operations = _operations.Values
            .Where(o => o.CreatedAt.Year == year && o.CreatedAt.Month == month)
            .ToList();
        
        return Task.FromResult(new TreasuryReport
        {
            ReportDate = new DateTime(year, month, 1),
            TotalOperations = operations.Count,
            TotalDisbursements = operations.Where(o => o.Type == TreasuryOperationType.Disbursement).Sum(o => o.Amount),
            TotalIssuances = operations.Where(o => o.Type == TreasuryOperationType.Issuance).Sum(o => o.Amount),
            PendingOperations = operations.Count(o => o.Status == TreasuryOperationStatus.Pending || o.Status == TreasuryOperationStatus.AwaitingApproval),
            CompletedOperations = operations.Count(o => o.Status == TreasuryOperationStatus.Completed)
        });
    }
}

// DTOs
public record CreateOperationRequest(
    TreasuryOperationType Type,
    decimal Amount,
    string Currency,
    string InitiatedBy,
    string? FromAccount = null,
    string? ToAccount = null,
    string? BeneficiaryId = null,
    string? BeneficiaryName = null,
    string? Description = null,
    string? Category = null,
    string? TreatyReference = null,
    bool RequiresTreatyCompliance = false
);

public record ApprovalRequest(
    string ApproverId,
    string ApproverName,
    string ApproverRole,
    ApprovalDecision Decision,
    string? Comments = null,
    string? Signature = null
);

public record CreateIssuanceRequest(
    IssuanceType Type,
    string CurrencySymbol,
    decimal Amount,
    string RecipientAccount,
    string AuthorizedBy,
    string? BackingAsset = null,
    decimal? BackingAmount = null,
    string? TreatyReference = null
);

public record CreateAccountRequest(
    string Name,
    TreasuryAccountType Type,
    decimal DailyLimit,
    decimal MonthlyLimit,
    int RequiredSignatures,
    List<string> Signatories
);

public record OperationFilter(
    TreasuryOperationType? Type = null,
    TreasuryOperationStatus? Status = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null
);

public record TreasuryReport
{
    public DateTime ReportDate { get; init; }
    public int TotalOperations { get; init; }
    public decimal TotalDisbursements { get; init; }
    public decimal TotalIssuances { get; init; }
    public int PendingOperations { get; init; }
    public int CompletedOperations { get; init; }
}
