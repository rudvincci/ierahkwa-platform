using Common.Application.DTOs;
using SmartAccounting.Application.DTOs;

namespace SmartAccounting.Application.Interfaces;

public interface IUnitService
{
    Task<UnitDto?> GetByIdAsync(int id);
    Task<PagedResult<UnitDto>> GetAllAsync(QueryParameters parameters);
    Task<IEnumerable<UnitDto>> GetAllActiveAsync();
    Task<UnitDto> CreateAsync(CreateUnitDto dto);
    Task<UnitDto> UpdateAsync(int id, CreateUnitDto dto);
    Task<bool> DeleteAsync(int id);
}

public interface ICategoryService
{
    Task<CategoryDto?> GetByIdAsync(int id);
    Task<PagedResult<CategoryDto>> GetAllAsync(QueryParameters parameters);
    Task<IEnumerable<CategoryDto>> GetTreeAsync();
    Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
    Task<CategoryDto> UpdateAsync(int id, CreateCategoryDto dto);
    Task<bool> DeleteAsync(int id);
}

public interface IProductService
{
    Task<ProductDto?> GetByIdAsync(int id);
    Task<ProductDto?> GetByCodeAsync(string code);
    Task<ProductDto?> GetByBarcodeAsync(string barcode);
    Task<PagedResult<ProductDto>> GetAllAsync(QueryParameters parameters);
    Task<IEnumerable<ProductDto>> GetByCategoryAsync(int categoryId);
    Task<IEnumerable<ProductDto>> GetLowStockAsync();
    Task<ProductDto> CreateAsync(CreateProductDto dto);
    Task<ProductDto> UpdateAsync(int id, CreateProductDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> UpdateStockAsync(int id, decimal quantity);
}

public interface ISupplierService
{
    Task<SupplierDto?> GetByIdAsync(int id);
    Task<PagedResult<SupplierDto>> GetAllAsync(QueryParameters parameters);
    Task<SupplierDto?> GetDefaultAsync();
    Task<SupplierDto> CreateAsync(CreateSupplierDto dto);
    Task<SupplierDto> UpdateAsync(int id, CreateSupplierDto dto);
    Task<bool> DeleteAsync(int id);
}

public interface ISchoolYearService
{
    Task<SchoolYearDto?> GetByIdAsync(int id);
    Task<SchoolYearDto?> GetCurrentAsync();
    Task<PagedResult<SchoolYearDto>> GetAllAsync(QueryParameters parameters);
    Task<SchoolYearDto> CreateAsync(CreateSchoolYearDto dto);
    Task<SchoolYearDto> UpdateAsync(int id, CreateSchoolYearDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> SetCurrentAsync(int id);
}

public interface IFeesService
{
    Task<FeesDto?> GetByIdAsync(int id);
    Task<PagedResult<FeesDto>> GetAllAsync(QueryParameters parameters);
    Task<IEnumerable<FeesDto>> GetBySchoolYearAsync(int schoolYearId);
    Task<IEnumerable<FeesDto>> GetByGradeAsync(int gradeId);
    Task<FeesDto> CreateAsync(CreateFeesDto dto);
    Task<FeesDto> UpdateAsync(int id, CreateFeesDto dto);
    Task<bool> DeleteAsync(int id);
}
