using NET10.Core.Interfaces;
using NET10.Core.Models;

namespace NET10.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly List<Token> _tokens;
    private readonly Dictionary<string, List<TokenBalance>> _balances;

    public TokenService()
    {
        // Initialize with default Ierahkwa tokens
        _tokens = new List<Token>
        {
            // Native tokens
            new Token { Id = "igt-main", Symbol = "IGT", Name = "Ierahkwa Government Token", Decimals = 18, Price = 1.0m, IsNative = true, IsVerified = true, TotalSupply = 1000000000, Address = "0xIGT...Native" },
            new Token { Id = "igt-pm", Symbol = "IGT-PM", Name = "Prime Minister Token", Decimals = 18, Price = 1.0m, IsVerified = true, TotalSupply = 100000000, Address = "0xIGT...PM" },
            new Token { Id = "igt-bdet", Symbol = "IGT-BDET", Name = "Central Bank Token", Decimals = 18, Price = 1.0m, IsVerified = true, TotalSupply = 500000000, Address = "0xIGT...BDET" },
            new Token { Id = "igt-defi", Symbol = "IGT-DEFI", Name = "DeFi Governance Token", Decimals = 18, Price = 0.5m, IsVerified = true, TotalSupply = 100000000, Address = "0xIGT...DEFI" },
            new Token { Id = "igt-net", Symbol = "IGT-NET", Name = "Network Domain Token", Decimals = 9, Price = 0.1m, IsVerified = true, TotalSupply = 10000000000, Address = "0xIGT...NET" },
            
            // Stablecoins
            new Token { Id = "usdt", Symbol = "USDT", Name = "Tether USD", Decimals = 6, Price = 1.0m, IsVerified = true, TotalSupply = 100000000000, Address = "0xdAC17F958D2ee523a2206206994597C13D831ec7" },
            new Token { Id = "usdc", Symbol = "USDC", Name = "USD Coin", Decimals = 6, Price = 1.0m, IsVerified = true, TotalSupply = 50000000000, Address = "0xA0b86991c6218b36c1d19D4a2e9Eb0cE3606eB48" },
            new Token { Id = "igt-stable", Symbol = "IGTS", Name = "Ierahkwa Stable Dollar", Decimals = 18, Price = 1.0m, IsVerified = true, TotalSupply = 1000000000, Address = "0xIGT...STABLE" },
            
            // Major cryptocurrencies
            new Token { Id = "wbtc", Symbol = "WBTC", Name = "Wrapped Bitcoin", Decimals = 8, Price = 65000.0m, IsVerified = true, TotalSupply = 21000000, Address = "0x2260FAC5E5542a773Aa44fBCfeDf7C193bc2C599" },
            new Token { Id = "weth", Symbol = "WETH", Name = "Wrapped Ethereum", Decimals = 18, Price = 3500.0m, IsVerified = true, TotalSupply = 120000000, Address = "0xC02aaA39b223FE8D0A0e5C4F27eAD9083C756Cc2" },
            new Token { Id = "wbnb", Symbol = "WBNB", Name = "Wrapped BNB", Decimals = 18, Price = 550.0m, IsVerified = true, TotalSupply = 200000000, Address = "0xbb4CdB9CBd36B01bD1cBaEBF2De08d9173bc095c" },
            
            // Additional IGT tokens
            new Token { Id = "igt-trade", Symbol = "IGT-TRADE", Name = "Trading Token", Decimals = 18, Price = 0.25m, IsVerified = true, TotalSupply = 500000000, Address = "0xIGT...TRADE" },
            new Token { Id = "igt-stake", Symbol = "IGT-STAKE", Name = "Staking Token", Decimals = 18, Price = 0.15m, IsVerified = true, TotalSupply = 200000000, Address = "0xIGT...STAKE" },
            new Token { Id = "igt-liq", Symbol = "IGT-LIQ", Name = "Liquidity Token", Decimals = 18, Price = 0.08m, IsVerified = true, TotalSupply = 1000000000, Address = "0xIGT...LIQ" },
        };

        _balances = new Dictionary<string, List<TokenBalance>>();
    }

    public Task<List<Token>> GetAllTokensAsync()
    {
        return Task.FromResult(_tokens.ToList());
    }

    public Task<Token?> GetTokenByIdAsync(string tokenId)
    {
        return Task.FromResult(_tokens.FirstOrDefault(t => t.Id == tokenId));
    }

    public Task<Token?> GetTokenBySymbolAsync(string symbol)
    {
        return Task.FromResult(_tokens.FirstOrDefault(t => 
            t.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase)));
    }

    public Task<Token?> GetTokenByAddressAsync(string address)
    {
        return Task.FromResult(_tokens.FirstOrDefault(t => 
            t.Address.Equals(address, StringComparison.OrdinalIgnoreCase)));
    }

    public Task<List<Token>> SearchTokensAsync(string query)
    {
        var results = _tokens.Where(t =>
            t.Symbol.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            t.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            t.Address.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();
        return Task.FromResult(results);
    }

    public Task<Token> AddTokenAsync(Token token)
    {
        token.Id = Guid.NewGuid().ToString();
        token.CreatedAt = DateTime.UtcNow;
        _tokens.Add(token);
        return Task.FromResult(token);
    }

    public Task<Token> UpdateTokenAsync(Token token)
    {
        var existing = _tokens.FirstOrDefault(t => t.Id == token.Id);
        if (existing != null)
        {
            var index = _tokens.IndexOf(existing);
            _tokens[index] = token;
        }
        return Task.FromResult(token);
    }

    public Task<bool> RemoveTokenAsync(string tokenId)
    {
        var token = _tokens.FirstOrDefault(t => t.Id == tokenId);
        if (token != null)
        {
            _tokens.Remove(token);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<List<TokenBalance>> GetUserBalancesAsync(string userId)
    {
        if (!_balances.ContainsKey(userId))
        {
            // Initialize with demo balances
            _balances[userId] = _tokens.Select(t => new TokenBalance
            {
                UserId = userId,
                TokenId = t.Id,
                Token = t,
                Balance = GetDemoBalance(t.Symbol),
                UsdValue = GetDemoBalance(t.Symbol) * t.Price
            }).ToList();
        }
        return Task.FromResult(_balances[userId]);
    }

    public Task<decimal> GetTokenPriceAsync(string tokenId)
    {
        var token = _tokens.FirstOrDefault(t => t.Id == tokenId);
        return Task.FromResult(token?.Price ?? 0);
    }

    private decimal GetDemoBalance(string symbol)
    {
        return symbol switch
        {
            "IGT" => 10000,
            "IGT-PM" => 5000,
            "USDT" => 10000,
            "USDC" => 5000,
            "IGTS" => 5000,
            "WETH" => 2.5m,
            "WBTC" => 0.15m,
            "WBNB" => 10,
            _ => 1000
        };
    }
}
