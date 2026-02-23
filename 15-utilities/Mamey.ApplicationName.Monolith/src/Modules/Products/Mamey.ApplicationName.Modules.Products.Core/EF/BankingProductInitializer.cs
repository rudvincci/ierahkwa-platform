using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Mamey.Bank.Modules.BankingProducts.Core.Entities;
using Mamey.MicroMonolith.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Mamey.ApplicationName.Modules.Products.Core.EF;

internal sealed class BankingProductInitializer : IInitializer
{
    private readonly BankingProductDbContext _dbContext;
    private readonly ILogger<BankingProductInitializer> _logger;

    public BankingProductInitializer(BankingProductDbContext dbContext, ILogger<BankingProductInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Initializes the banking products module by seeding all required data.
    /// </summary>
    public async Task InitAsync(IServiceProvider serviceProvider)
    {
        _logger.LogInformation("Initializing banking products...");

        if (await _dbContext.Products.AnyAsync())
        {
            _logger.LogInformation("Banking products already initialized.");
            return;
        }

        await SeedBankingProductsFromJsonAsync();
        _logger.LogInformation("Banking products initialized.");
    }

    /// <summary>
    /// Seeds banking products from a JSON file.
    /// </summary>
    private async Task SeedBankingProductsFromJsonAsync()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "Resources", "banking-products.seed.json");
        if (!File.Exists(filePath))
        {
            _logger.LogError($"File not found: {filePath}");
            return;
        }

        var jsonData = await File.ReadAllTextAsync(filePath);
        var bankingProducts = JsonSerializer.Deserialize<List<Product>>(jsonData, Mamey.JsonExtensions.SerializerOptions);

        if (bankingProducts == null || bankingProducts.Count == 0)
        {
            _logger.LogWarning("No banking products found in the JSON file.");
            return;
        }

        await _dbContext.Products.AddRangeAsync(bankingProducts);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"{bankingProducts.Count} banking products seeded from JSON.");
    }
}
