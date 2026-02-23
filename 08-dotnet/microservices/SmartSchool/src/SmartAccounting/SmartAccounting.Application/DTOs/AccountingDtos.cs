using SmartAccounting.Domain.Entities;

namespace SmartAccounting.Application.DTOs;

// Unit DTOs
public class UnitDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public bool IsActive { get; set; }
}

public class CreateUnitDto
{
    public string Name { get; set; } = string.Empty;
    public string? ShortName { get; set; }
}

// Category DTOs
public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentId { get; set; }
    public string? ParentName { get; set; }
    public bool IsActive { get; set; }
    public IEnumerable<CategoryDto> Children { get; set; } = new List<CategoryDto>();
}

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentId { get; set; }
}

// Product DTOs
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Barcode { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int UnitId { get; set; }
    public string? UnitName { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal Quantity { get; set; }
    public decimal MinQuantity { get; set; }
    public string? Image { get; set; }
    public bool IsActive { get; set; }
}

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Barcode { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public int UnitId { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal Quantity { get; set; } = 0;
    public decimal MinQuantity { get; set; } = 0;
}

// Supplier DTOs
public class SupplierDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? TaxNumber { get; set; }
    public decimal Balance { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
}

public class CreateSupplierDto
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? TaxNumber { get; set; }
    public bool IsDefault { get; set; } = false;
}

// SchoolYear DTOs
public class SchoolYearDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public bool IsActive { get; set; }
}

public class CreateSchoolYearDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsCurrent { get; set; } = false;
}

// Fees DTOs
public class FeesDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public int SchoolYearId { get; set; }
    public string? SchoolYearName { get; set; }
    public int? GradeId { get; set; }
    public string? GradeName { get; set; }
    public FeeType Type { get; set; }
    public bool IsMandatory { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsActive { get; set; }
}

public class CreateFeesDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public int SchoolYearId { get; set; }
    public int? GradeId { get; set; }
    public FeeType Type { get; set; } = FeeType.Tuition;
    public bool IsMandatory { get; set; } = true;
    public DateTime? DueDate { get; set; }
}
