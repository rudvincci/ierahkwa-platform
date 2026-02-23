using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using OutlookExtractor.Core.Interfaces;
using OutlookExtractor.Core.Models;

namespace OutlookExtractor.Infrastructure.Services;

/// <summary>
/// Export Service Implementation
/// Exports extracted emails to Text and Excel files
/// IERAHKWA Platform - Sovereign Government Tool
/// </summary>
public class ExportService : IExportService
{
    public async Task<ExportResult> ExportToTextAsync(List<ExtractedEmail> emails, string outputPath)
    {
        try
        {
            var sb = new StringBuilder();
            sb.AppendLine("======================================================");
            sb.AppendLine("   IERAHKWA EMAIL EXTRACTOR - TEXT EXPORT");
            sb.AppendLine("   Sovereign Government of Ierahkwa Ne Kanienke");
            sb.AppendLine($"   Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
            sb.AppendLine("======================================================");
            sb.AppendLine();
            sb.AppendLine($"Total Emails: {emails.Count}");
            sb.AppendLine($"Unique Emails: {emails.Select(e => e.EmailAddress).Distinct().Count()}");
            sb.AppendLine();
            sb.AppendLine("EMAIL ADDRESSES:");
            sb.AppendLine("------------------------------------------------------");
            sb.AppendLine();

            foreach (var email in emails.OrderBy(e => e.EmailAddress))
            {
                sb.AppendLine($"{email.EmailAddress}");
                if (!string.IsNullOrEmpty(email.DisplayName))
                {
                    sb.AppendLine($"  Name: {email.DisplayName}");
                }
                sb.AppendLine($"  Source: {email.Source}");
                sb.AppendLine($"  Frequency: {email.Frequency}");
                
                if (email.Metadata != null)
                {
                    if (!string.IsNullOrEmpty(email.Metadata.Company))
                        sb.AppendLine($"  Company: {email.Metadata.Company}");
                    if (!string.IsNullOrEmpty(email.Metadata.JobTitle))
                        sb.AppendLine($"  Job Title: {email.Metadata.JobTitle}");
                }
                
                sb.AppendLine();
            }

            sb.AppendLine("======================================================");
            sb.AppendLine($"Extraction completed: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
            sb.AppendLine("======================================================");

            await File.WriteAllTextAsync(outputPath, sb.ToString(), Encoding.UTF8);

            return new ExportResult
            {
                Success = true,
                TextFilePath = outputPath,
                RecordsExported = emails.Count,
                Message = "Successfully exported to text file"
            };
        }
        catch (Exception ex)
        {
            return new ExportResult
            {
                Success = false,
                Message = $"Error exporting to text: {ex.Message}"
            };
        }
    }

    public async Task<ExportResult> ExportToExcelAsync(List<ExtractedEmail> emails, string outputPath)
    {
        try
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Email Addresses");

            // Header styling
            worksheet.Cell(1, 1).Value = "IERAHKWA EMAIL EXTRACTOR";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Cell(1, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#1E3A8A");
            worksheet.Cell(1, 1).Style.Font.FontColor = XLColor.White;
            worksheet.Range(1, 1, 1, 10).Merge();

            worksheet.Cell(2, 1).Value = "Sovereign Government of Ierahkwa Ne Kanienke";
            worksheet.Cell(2, 1).Style.Font.Italic = true;
            worksheet.Range(2, 1, 2, 10).Merge();

            worksheet.Cell(3, 1).Value = $"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";
            worksheet.Range(3, 1, 3, 10).Merge();

            // Column headers
            int headerRow = 5;
            var headers = new[] { "Email Address", "Display Name", "Source", "Frequency", "Company", "Job Title", "Phone", "Subject/Event", "Date", "Folder/Location" };
            
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(headerRow, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#3B82F6");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            // Data rows
            int row = headerRow + 1;
            foreach (var email in emails.OrderBy(e => e.EmailAddress))
            {
                worksheet.Cell(row, 1).Value = email.EmailAddress;
                worksheet.Cell(row, 2).Value = email.DisplayName;
                worksheet.Cell(row, 3).Value = email.Source;
                worksheet.Cell(row, 4).Value = email.Frequency;
                
                if (email.Metadata != null)
                {
                    worksheet.Cell(row, 5).Value = email.Metadata.Company ?? "";
                    worksheet.Cell(row, 6).Value = email.Metadata.JobTitle ?? "";
                    worksheet.Cell(row, 7).Value = email.Metadata.PhoneNumber ?? "";
                    worksheet.Cell(row, 8).Value = email.Metadata.Subject ?? email.Metadata.EventTitle ?? "";
                    worksheet.Cell(row, 9).Value = email.Metadata.MessageDate?.ToString("yyyy-MM-dd") ?? 
                                                    email.Metadata.EventDate?.ToString("yyyy-MM-dd") ?? "";
                    worksheet.Cell(row, 10).Value = email.Metadata.FolderPath ?? "";
                }
                
                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            // Summary section
            int summaryRow = row + 2;
            worksheet.Cell(summaryRow, 1).Value = "SUMMARY";
            worksheet.Cell(summaryRow, 1).Style.Font.Bold = true;
            worksheet.Cell(summaryRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#DBEAFE");
            
            worksheet.Cell(summaryRow + 1, 1).Value = "Total Records:";
            worksheet.Cell(summaryRow + 1, 2).Value = emails.Count;
            
            worksheet.Cell(summaryRow + 2, 1).Value = "Unique Emails:";
            worksheet.Cell(summaryRow + 2, 2).Value = emails.Select(e => e.EmailAddress).Distinct().Count();

            await Task.Run(() => workbook.SaveAs(outputPath));

            return new ExportResult
            {
                Success = true,
                ExcelFilePath = outputPath,
                RecordsExported = emails.Count,
                Message = "Successfully exported to Excel file"
            };
        }
        catch (Exception ex)
        {
            return new ExportResult
            {
                Success = false,
                Message = $"Error exporting to Excel: {ex.Message}"
            };
        }
    }

    public async Task<ExportResult> ExportAsync(List<ExtractedEmail> emails, string outputDirectory, ExportFormat format)
    {
        try
        {
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var result = new ExportResult { Success = true };

            if (format == ExportFormat.Text || format == ExportFormat.Both)
            {
                var textPath = Path.Combine(outputDirectory, $"ierahkwa_emails_{timestamp}.txt");
                var textResult = await ExportToTextAsync(emails, textPath);
                result.TextFilePath = textResult.TextFilePath;
                result.Success = result.Success && textResult.Success;
            }

            if (format == ExportFormat.Excel || format == ExportFormat.Both)
            {
                var excelPath = Path.Combine(outputDirectory, $"ierahkwa_emails_{timestamp}.xlsx");
                var excelResult = await ExportToExcelAsync(emails, excelPath);
                result.ExcelFilePath = excelResult.ExcelFilePath;
                result.Success = result.Success && excelResult.Success;
            }

            result.RecordsExported = emails.Count;
            result.Message = result.Success ? "Export completed successfully" : "Export completed with errors";

            return result;
        }
        catch (Exception ex)
        {
            return new ExportResult
            {
                Success = false,
                Message = $"Error during export: {ex.Message}"
            };
        }
    }
}
