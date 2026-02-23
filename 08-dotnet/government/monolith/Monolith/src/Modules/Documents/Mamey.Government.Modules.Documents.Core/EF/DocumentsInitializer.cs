using System.Linq;
using Mamey.Government.Modules.Documents.Core.Domain.Entities;
using Mamey.Government.Modules.Documents.Core.Domain.Repositories;
using Mamey.Government.Modules.Documents.Core.Domain.ValueObjects;
using Mamey.Government.Shared.Abstractions;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Documents.Core.EF;

internal class DocumentsInitializer : IInitializer
{
    private readonly IDocumentRepository _documentRepository;
    private readonly ILogger<DocumentsInitializer> _logger;
    
    private static readonly TenantId TenantId = new(SeedData.TenantId);

    private static readonly string[] DocumentCategories = {
        "Passport", "BirthCertificate", "DriverLicense", "IDCard", "ProofOfResidence",
        "MarriageCertificate", "DivorceCertificate", "EducationCertificate", "EmploymentLetter",
        "TaxDocument", "BankStatement", "UtilityBill", "MedicalRecord", "LegalDocument"
    };

    private static readonly string[] ContentTypes = {
        "application/pdf", "image/jpeg", "image/png", "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
    };

    public DocumentsInitializer(
        IDocumentRepository documentRepository,
        ILogger<DocumentsInitializer> logger)
    {
        _documentRepository = documentRepository;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting documents database initialization...");

        // Check if data already exists
        var existingDocuments = await _documentRepository.BrowseAsync(cancellationToken);

        if (existingDocuments.Any())
        {
            _logger.LogInformation("Database already contains {Count} documents. Skipping seed.", 
                existingDocuments.Count);
            return;
        }

        var random = new Random(42); // Fixed seed for reproducible data
        var documents = new List<Document>();

        for (int i = 1; i <= 100; i++)
        {
            var documentId = new DocumentId(SeedData.GenerateDeterministicGuid(i, "document"));
            var category = DocumentCategories[random.Next(DocumentCategories.Length)];
            var fileName = $"{category}_{i:D4}.pdf";
            var contentType = ContentTypes[random.Next(ContentTypes.Length)];
            var fileSize = random.Next(100000, 5000000); // 100KB to 5MB
            var storageBucket = "documents";
            var storageKey = $"documents/{SeedData.TenantId:N}/{documentId.Value:N}/{fileName}";
            
            var document = new Document(
                documentId,
                TenantId,
                fileName,
                contentType,
                fileSize,
                storageBucket,
                storageKey,
                category);
            
            // Add description to some documents
            if (random.Next(100) < 60)
            {
                document.UpdateDescription($"Document {i}: {category} for tenant {TenantId:N}");
            }
            
            // Delete 5% of documents
            if (random.Next(100) < 5)
            {
                document.Delete();
            }

            documents.Add(document);
        }

        _logger.LogInformation("Created {Count} mock documents", documents.Count);

        // Add documents using repository
        foreach (var document in documents)
        {
            await _documentRepository.AddAsync(document, cancellationToken);
        }
        
        _logger.LogInformation("Successfully seeded {Count} documents", documents.Count);
    }
}
