using System.Reflection;
using ClosedXML.Excel;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Excel;

public static class Extensions
{
    public static IServiceCollection AddExcel(this IServiceCollection services)
    {
        return services;
    }
}

public interface IExcelExporter
{
    void ExportMultipleSheets<T>(IDictionary<string, IEnumerable<T>> workbookSheets,
        string filePath, bool capitalizedHeaders = false);
    void Export<T>(IEnumerable<T> items, string sheetName, string filePath, bool capitalizedHeaders = false);
}

public class ExcelExporter : IExcelExporter
{
    public void ExportMultipleSheets<T>(IDictionary<string, IEnumerable<T>> workbookSheets, string filePath, bool capitalizedHeaders = false)
    {
        using (var workbook = new XLWorkbook())
        {
            foreach (var sheet in workbookSheets)
            {
                var key = sheet.Key;
                var sheetData = sheet.Value;
                var typeOfT = sheetData.GetType().GetGenericArguments()[0];
                var addDataMethod = typeof(ExcelExporter).GetMethod("AddDataToWorksheet", BindingFlags.NonPublic | BindingFlags.Instance)
                                                         .MakeGenericMethod(typeOfT);
                                                         //.MakeGenericMethod(typeof(T));
                addDataMethod.Invoke(this, new object[] { workbook, sheetData, key, capitalizedHeaders });
            }
            workbook.SaveAs(filePath);
        }
    }

    public void Export<T>(IEnumerable<T> items, string sheetName, string filePath, bool capitalizedHeaders = false)
    {
        using (var workbook = new XLWorkbook())
        {
            AddDataToWorksheet(workbook, items, sheetName, capitalizedHeaders);
            workbook.SaveAs(filePath);
        }
    }

    private void AddDataToWorksheet<T>(XLWorkbook workbook, IEnumerable<T> items, string sheetName, bool capitalizedHeaders = false)
    {
        try
        {
            var worksheet = workbook.Worksheets.Add(sheetName);
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                      .Where(p => p.GetCustomAttribute<ExcelExportIgnore>() == null)
                                      .OrderBy(c => c.Name)
                                      .ToArray();

            CreateHeaderRow(worksheet, properties, capitalizedHeaders);

            int rowNumber = 2;
            foreach (var item in items)
            {
                CreateDataRow(worksheet, properties, item, rowNumber);
                rowNumber++;
            }

            worksheet.Columns().AdjustToContents();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private void CreateHeaderRow(IXLWorksheet worksheet, PropertyInfo[] properties, bool capitalizedHeaders)
    {
        int columnIndex = 1;
        foreach (var property in properties)
        {
            if (property.PropertyType.IsClass && !property.PropertyType.IsPrimitive && property.PropertyType != typeof(string))
            {
                var nestedProperties = property.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                            .Where(p => p.GetCustomAttribute<ExcelExportIgnore>() == null)
                                                            .OrderBy(c=> c.Name)
                                                            .ToArray();
                foreach (var nestedProperty in nestedProperties)
                {
                    if (nestedProperty.PropertyType.IsClass && !nestedProperty.PropertyType.IsPrimitive && nestedProperty.PropertyType != typeof(string))
                    {
                        var subNestedProperties = nestedProperty.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                                                .Where(p => p.GetCustomAttribute<ExcelExportIgnore>() == null)
                                                                                .OrderBy(c => c.Name)
                                                                                .ToArray();
                        foreach (var subNestedProperty in subNestedProperties)
                        {
                            worksheet.Cell(1, columnIndex).Value = capitalizedHeaders ? subNestedProperty.Name.ToUpper() : subNestedProperty.Name;
                            worksheet.Cell(1, columnIndex).Style.Font.Bold = true;
                            columnIndex++;
                        }
                    }
                    else
                    {
                        worksheet.Cell(1, columnIndex).Value = capitalizedHeaders ? $"{property.Name}.{nestedProperty.Name}".ToUpper() : $"{property.Name}.{nestedProperty.Name}";
                        worksheet.Cell(1, columnIndex).Style.Font.Bold = true;
                        columnIndex++;
                    }
                }
            }
            else
            {
                worksheet.Cell(1, columnIndex).Value = capitalizedHeaders ? property.Name.ToUpper() : property.Name;
                worksheet.Cell(1, columnIndex).Style.Font.Bold = true;
            }
                columnIndex++;
        }
    }

    private void CreateDataRow<T>(IXLWorksheet worksheet, PropertyInfo[] properties, T item, int rowNumber)
    {
        AddNestedPropertiesToWorksheet(worksheet, properties, item, rowNumber);
    }

    private void AddNestedPropertiesToWorksheet<T>(IXLWorksheet worksheet, PropertyInfo[] properties, T item, int rowNumber)
    {
        try
        {
            int columnIndex = 1;
            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(item);
                if (property.PropertyType == typeof(DateTime))
                {
                    worksheet.Cell(rowNumber, columnIndex).Value = propertyValue is null ? string.Empty : ((DateTime)propertyValue).ToShortDateString();
                }
                else if (property.PropertyType.IsEnum)
                {
                    worksheet.Cell(rowNumber, columnIndex).Value = propertyValue?.ToString() ?? string.Empty;
                }
                else if (property.PropertyType.IsClass && !property.PropertyType.IsPrimitive && property.PropertyType != typeof(string))
                {
                    var nestedProperties = property.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                                .Where(p => p.GetCustomAttribute<ExcelExportIgnore>() == null)
                                                                .ToArray();
                    foreach (var nestedProperty in nestedProperties)
                    {
                        var subValue = propertyValue == null ? null : nestedProperty.GetValue(propertyValue);
                        if (subValue != null && nestedProperty.PropertyType.IsClass && !nestedProperty.PropertyType.IsPrimitive && nestedProperty.PropertyType != typeof(string))
                        {
                            var subNestedProperties = nestedProperty.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                                                .Where(p => p.GetCustomAttribute<ExcelExportIgnore>() == null)
                                                                                .OrderBy(c => c.Name)
                                                                                .ToArray();
                            foreach (var subNestedProperty in subNestedProperties)
                            {
                                var subNestedValue = subNestedProperty.GetValue(subValue);
                                worksheet.Cell(rowNumber, columnIndex).Value = subNestedValue?.ToString() ?? string.Empty;
                                columnIndex++;
                            }
                        }
                        else
                        {
                            worksheet.Cell(rowNumber, columnIndex).Value = subValue?.ToString() ?? string.Empty;
                            //columnIndex++;
                        }
                    }
                }
                else
                {
                    worksheet.Cell(rowNumber, columnIndex).Value = propertyValue?.ToString() ?? string.Empty;
                }
                columnIndex++;
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
