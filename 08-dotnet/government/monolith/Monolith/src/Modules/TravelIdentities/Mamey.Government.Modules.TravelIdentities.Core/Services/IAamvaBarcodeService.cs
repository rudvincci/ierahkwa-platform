using System;

namespace Mamey.Government.Modules.TravelIdentities.Core.Services;

/// <summary>
/// Service for generating AAMVA PDF417 barcodes for driver's licenses and ID cards.
/// Based on AAMVA Card Design Standard (CDS) v2020.
/// </summary>
public interface IAamvaBarcodeService
{
    /// <summary>
    /// Generates AAMVA-compliant data for PDF417 barcode.
    /// </summary>
    AamvaData GenerateBarcodeData(AamvaInput input);
    
    /// <summary>
    /// Generates a PDF417 barcode image from AAMVA data.
    /// </summary>
    byte[] GenerateBarcodeImage(AamvaData data, int width = 400, int height = 150);
    
    /// <summary>
    /// Parses AAMVA data from a scanned barcode.
    /// </summary>
    AamvaInput? ParseBarcodeData(string barcodeData);
}

public record AamvaInput(
    string DocumentNumber,
    string Jurisdiction,
    string FirstName,
    string LastName,
    string? MiddleName,
    DateTime DateOfBirth,
    string Sex,
    string EyeColor,
    int HeightInches,
    string StreetAddress,
    string City,
    string State,
    string PostalCode,
    DateTime IssueDate,
    DateTime ExpirationDate,
    string DocumentClass,
    string? Restrictions = null,
    string? Endorsements = null);

public record AamvaData(
    string RawData,
    string Header,
    string Subfiles);
