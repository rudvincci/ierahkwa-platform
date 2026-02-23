using System.Reactive;
using ReactiveUI;
using System.Reactive.Linq;
using Mamey.Portal.Citizenship.Application.Models;
using Mamey.Portal.Web.Public;

namespace Mamey.Portal.Web.ViewModels.Public;

public sealed class ValidateCitizenViewModel : ReactiveObject
{
    private readonly IPublicValidationApiClient _api;

    private string _documentNumber = string.Empty;
    public string DocumentNumber
    {
        get => _documentNumber;
        set => this.RaiseAndSetIfChanged(ref _documentNumber, value);
    }

    private PublicDocumentValidationResult? _result;
    public PublicDocumentValidationResult? Result
    {
        get => _result;
        private set => this.RaiseAndSetIfChanged(ref _result, value);
    }

    private string _message = string.Empty;
    public string Message
    {
        get => _message;
        private set => this.RaiseAndSetIfChanged(ref _message, value);
    }

    public bool CanValidate => !string.IsNullOrWhiteSpace(DocumentNumber);

    public ReactiveCommand<Unit, Unit> Validate { get; }
    public ReactiveCommand<Stream, Unit> ScanBarcode { get; }

    private BarcodeScanResult? _scanResult;
    public BarcodeScanResult? ScanResult
    {
        get => _scanResult;
        private set => this.RaiseAndSetIfChanged(ref _scanResult, value);
    }

    public ValidateCitizenViewModel(IPublicValidationApiClient api)
    {
        _api = api;
        var canValidate = this.WhenAnyValue(x => x.DocumentNumber, n => !string.IsNullOrWhiteSpace(n));

        Validate = ReactiveCommand.CreateFromTask(async ct =>
        {
            try
            {
                Result = null;
                Message = string.Empty;

                var res = await _api.ValidateAsync(DocumentNumber, ct);
                Result = res;

                if (!res.Found)
                {
                    Message = "Not found. (This page does not display private citizen data.)";
                    return;
                }

                Message = res.IsValid
                    ? "Valid document. (This page does not display private citizen data.)"
                    : $"Document found but not valid: {res.Status}.";
            }
            catch (Exception ex)
            {
                Result = null;
                Message = ex.Message;
            }
        }, canValidate);

        ScanBarcode = ReactiveCommand.CreateFromTask<Stream>(async (imageStream, ct) =>
        {
            try
            {
                ScanResult = null;
                Message = string.Empty;

                var result = await _api.ScanBarcodeFromImageAsync(imageStream, "id-card.jpg", ct);
                ScanResult = result;

                if (!result.Success)
                {
                    Message = result.Message ?? "Failed to scan barcode from image.";
                    return;
                }

                if (result.DocumentNumber != null)
                {
                    // Auto-populate document number and validate
                    DocumentNumber = result.DocumentNumber;
                    if (result.Validation != null)
                    {
                        Result = result.Validation;
                        Message = result.Validation.IsValid
                            ? "Barcode scanned successfully. Document is valid. (This page does not display private citizen data.)"
                            : $"Barcode scanned. Document found but not valid: {result.Validation.Status}.";
                    }
                    else
                    {
                        // Trigger validation
                        await Validate.Execute();
                    }
                }
                else
                {
                    Message = result.Message ?? "Barcode scanned but document number could not be extracted.";
                }
            }
            catch (Exception ex)
            {
                ScanResult = null;
                Message = ex.Message;
            }
        });
    }

    public void Reset()
    {
        DocumentNumber = string.Empty;
        Result = null;
        ScanResult = null;
        Message = string.Empty;
    }
}


