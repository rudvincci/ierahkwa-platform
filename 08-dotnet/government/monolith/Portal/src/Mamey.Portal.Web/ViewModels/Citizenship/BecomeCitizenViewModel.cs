using System.Reactive;
using Microsoft.AspNetCore.Components.Forms;
using ReactiveUI;

namespace Mamey.Portal.Web.ViewModels.Citizenship;

public sealed class BecomeCitizenViewModel : ReactiveObject
{
    private string _firstName = string.Empty;
    public string FirstName
    {
        get => _firstName;
        set => this.RaiseAndSetIfChanged(ref _firstName, value);
    }

    private string _lastName = string.Empty;
    public string LastName
    {
        get => _lastName;
        set => this.RaiseAndSetIfChanged(ref _lastName, value);
    }

    private DateTime? _dateOfBirth;
    public DateTime? DateOfBirth
    {
        get => _dateOfBirth;
        set => this.RaiseAndSetIfChanged(ref _dateOfBirth, value);
    }

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set => this.RaiseAndSetIfChanged(ref _email, value);
    }

    private string _addressLine1 = string.Empty;
    public string AddressLine1
    {
        get => _addressLine1;
        set => this.RaiseAndSetIfChanged(ref _addressLine1, value);
    }

    private string _city = string.Empty;
    public string City
    {
        get => _city;
        set => this.RaiseAndSetIfChanged(ref _city, value);
    }

    private string _region = string.Empty;
    public string Region
    {
        get => _region;
        set => this.RaiseAndSetIfChanged(ref _region, value);
    }

    private string _postalCode = string.Empty;
    public string PostalCode
    {
        get => _postalCode;
        set => this.RaiseAndSetIfChanged(ref _postalCode, value);
    }

    // AAMVA-compliant fields
    private string _middleName = string.Empty;
    public string MiddleName
    {
        get => _middleName;
        set => this.RaiseAndSetIfChanged(ref _middleName, value);
    }

    private string _height = string.Empty; // e.g., "5'10" or "178cm"
    public string Height
    {
        get => _height;
        set => this.RaiseAndSetIfChanged(ref _height, value);
    }

    private string _eyeColor = string.Empty;
    public string EyeColor
    {
        get => _eyeColor;
        set => this.RaiseAndSetIfChanged(ref _eyeColor, value);
    }

    private string _hairColor = string.Empty;
    public string HairColor
    {
        get => _hairColor;
        set => this.RaiseAndSetIfChanged(ref _hairColor, value);
    }

    private string _sex = string.Empty; // M, F, X, or other
    public string Sex
    {
        get => _sex;
        set => this.RaiseAndSetIfChanged(ref _sex, value);
    }

    // Additional common citizenship application fields
    private string _phoneNumber = string.Empty;
    public string PhoneNumber
    {
        get => _phoneNumber;
        set => this.RaiseAndSetIfChanged(ref _phoneNumber, value);
    }

    private string _placeOfBirth = string.Empty;
    public string PlaceOfBirth
    {
        get => _placeOfBirth;
        set => this.RaiseAndSetIfChanged(ref _placeOfBirth, value);
    }

    private string _countryOfOrigin = string.Empty;
    public string CountryOfOrigin
    {
        get => _countryOfOrigin;
        set => this.RaiseAndSetIfChanged(ref _countryOfOrigin, value);
    }

    private string _maritalStatus = string.Empty;
    public string MaritalStatus
    {
        get => _maritalStatus;
        set => this.RaiseAndSetIfChanged(ref _maritalStatus, value);
    }

    private string _previousNames = string.Empty;
    public string PreviousNames
    {
        get => _previousNames;
        set => this.RaiseAndSetIfChanged(ref _previousNames, value);
    }

    // CIT-001-B: Treaty Acknowledgment
    private bool _acknowledgeTreaty = false;
    public bool AcknowledgeTreaty
    {
        get => _acknowledgeTreaty;
        set => this.RaiseAndSetIfChanged(ref _acknowledgeTreaty, value);
    }

    // CIT-001-C: Affidavit of Allegiance
    private bool _swearAllegiance = false;
    public bool SwearAllegiance
    {
        get => _swearAllegiance;
        set => this.RaiseAndSetIfChanged(ref _swearAllegiance, value);
    }

    private DateTime? _affidavitDate;
    public DateTime? AffidavitDate
    {
        get => _affidavitDate;
        set => this.RaiseAndSetIfChanged(ref _affidavitDate, value);
    }

    // CIT-001-D: Supporting Document Checklist
    private bool _hasBirthCertificate = false;
    public bool HasBirthCertificate
    {
        get => _hasBirthCertificate;
        set => this.RaiseAndSetIfChanged(ref _hasBirthCertificate, value);
    }

    private bool _hasPhotoId = false;
    public bool HasPhotoId
    {
        get => _hasPhotoId;
        set => this.RaiseAndSetIfChanged(ref _hasPhotoId, value);
    }

    private bool _hasProofOfResidence = false;
    public bool HasProofOfResidence
    {
        get => _hasProofOfResidence;
        set => this.RaiseAndSetIfChanged(ref _hasProofOfResidence, value);
    }

    private bool _hasBackgroundCheck = false;
    public bool HasBackgroundCheck
    {
        get => _hasBackgroundCheck;
        set => this.RaiseAndSetIfChanged(ref _hasBackgroundCheck, value);
    }

    // CIT-001-E: Biometric Enrollment Authorization
    private bool _authorizeBiometricEnrollment = false;
    public bool AuthorizeBiometricEnrollment
    {
        get => _authorizeBiometricEnrollment;
        set => this.RaiseAndSetIfChanged(ref _authorizeBiometricEnrollment, value);
    }

    // CIT-001-G: Declaration of Understanding
    private bool _declareUnderstanding = false;
    public bool DeclareUnderstanding
    {
        get => _declareUnderstanding;
        set => this.RaiseAndSetIfChanged(ref _declareUnderstanding, value);
    }

    // CIT-001-H: Consent to Verification and Data Processing
    private bool _consentToVerification = false;
    public bool ConsentToVerification
    {
        get => _consentToVerification;
        set => this.RaiseAndSetIfChanged(ref _consentToVerification, value);
    }

    private bool _consentToDataProcessing = false;
    public bool ConsentToDataProcessing
    {
        get => _consentToDataProcessing;
        set => this.RaiseAndSetIfChanged(ref _consentToDataProcessing, value);
    }

    private IReadOnlyList<IBrowserFile>? _personalDocuments;
    public IReadOnlyList<IBrowserFile>? PersonalDocuments
    {
        get => _personalDocuments;
        set => this.RaiseAndSetIfChanged(ref _personalDocuments, value);
    }

    private IReadOnlyList<IBrowserFile>? _passportPhoto;
    public IReadOnlyList<IBrowserFile>? PassportPhoto
    {
        get => _passportPhoto;
        set => this.RaiseAndSetIfChanged(ref _passportPhoto, value);
    }

    private IReadOnlyList<IBrowserFile>? _signatureImage;
    public IReadOnlyList<IBrowserFile>? SignatureImage
    {
        get => _signatureImage;
        set => this.RaiseAndSetIfChanged(ref _signatureImage, value);
    }

    private bool _useSignaturePad = true;
    public bool UseSignaturePad
    {
        get => _useSignaturePad;
        set => this.RaiseAndSetIfChanged(ref _useSignaturePad, value);
    }

    private string? _signatureDataUrl;
    public string? SignatureDataUrl
    {
        get => _signatureDataUrl;
        set => this.RaiseAndSetIfChanged(ref _signatureDataUrl, value);
    }

    // Emergency Contact Information
    private string _emergencyContactName = string.Empty;
    public string EmergencyContactName
    {
        get => _emergencyContactName;
        set => this.RaiseAndSetIfChanged(ref _emergencyContactName, value);
    }

    private string _emergencyContactRelationship = string.Empty;
    public string EmergencyContactRelationship
    {
        get => _emergencyContactRelationship;
        set => this.RaiseAndSetIfChanged(ref _emergencyContactRelationship, value);
    }

    private string _emergencyContactPhone = string.Empty;
    public string EmergencyContactPhone
    {
        get => _emergencyContactPhone;
        set => this.RaiseAndSetIfChanged(ref _emergencyContactPhone, value);
    }

    private string _emergencyContactEmail = string.Empty;
    public string EmergencyContactEmail
    {
        get => _emergencyContactEmail;
        set => this.RaiseAndSetIfChanged(ref _emergencyContactEmail, value);
    }

    private string _emergencyContactAddress = string.Empty;
    public string EmergencyContactAddress
    {
        get => _emergencyContactAddress;
        set => this.RaiseAndSetIfChanged(ref _emergencyContactAddress, value);
    }

    // Employment/Occupation Information
    private string _occupation = string.Empty;
    public string Occupation
    {
        get => _occupation;
        set => this.RaiseAndSetIfChanged(ref _occupation, value);
    }

    private string _employerName = string.Empty;
    public string EmployerName
    {
        get => _employerName;
        set => this.RaiseAndSetIfChanged(ref _employerName, value);
    }

    private string _employerAddress = string.Empty;
    public string EmployerAddress
    {
        get => _employerAddress;
        set => this.RaiseAndSetIfChanged(ref _employerAddress, value);
    }

    private string _employmentStartDate = string.Empty;
    public string EmploymentStartDate
    {
        get => _employmentStartDate;
        set => this.RaiseAndSetIfChanged(ref _employmentStartDate, value);
    }

    // Education Information
    private string _highestEducationLevel = string.Empty;
    public string HighestEducationLevel
    {
        get => _highestEducationLevel;
        set => this.RaiseAndSetIfChanged(ref _highestEducationLevel, value);
    }

    private string _schoolName = string.Empty;
    public string SchoolName
    {
        get => _schoolName;
        set => this.RaiseAndSetIfChanged(ref _schoolName, value);
    }

    private string _schoolLocation = string.Empty;
    public string SchoolLocation
    {
        get => _schoolLocation;
        set => this.RaiseAndSetIfChanged(ref _schoolLocation, value);
    }

    private string _graduationYear = string.Empty;
    public string GraduationYear
    {
        get => _graduationYear;
        set => this.RaiseAndSetIfChanged(ref _graduationYear, value);
    }

    // Family Information
    private string _spouseName = string.Empty;
    public string SpouseName
    {
        get => _spouseName;
        set => this.RaiseAndSetIfChanged(ref _spouseName, value);
    }

    private DateTime? _spouseDateOfBirth;
    public DateTime? SpouseDateOfBirth
    {
        get => _spouseDateOfBirth;
        set => this.RaiseAndSetIfChanged(ref _spouseDateOfBirth, value);
    }

    private string _spouseCitizenshipStatus = string.Empty;
    public string SpouseCitizenshipStatus
    {
        get => _spouseCitizenshipStatus;
        set => this.RaiseAndSetIfChanged(ref _spouseCitizenshipStatus, value);
    }

    private string _numberOfChildren = string.Empty;
    public string NumberOfChildren
    {
        get => _numberOfChildren;
        set => this.RaiseAndSetIfChanged(ref _numberOfChildren, value);
    }

    private string _parent1Name = string.Empty;
    public string Parent1Name
    {
        get => _parent1Name;
        set => this.RaiseAndSetIfChanged(ref _parent1Name, value);
    }

    private string _parent1CitizenshipStatus = string.Empty;
    public string Parent1CitizenshipStatus
    {
        get => _parent1CitizenshipStatus;
        set => this.RaiseAndSetIfChanged(ref _parent1CitizenshipStatus, value);
    }

    private string _parent2Name = string.Empty;
    public string Parent2Name
    {
        get => _parent2Name;
        set => this.RaiseAndSetIfChanged(ref _parent2Name, value);
    }

    private string _parent2CitizenshipStatus = string.Empty;
    public string Parent2CitizenshipStatus
    {
        get => _parent2CitizenshipStatus;
        set => this.RaiseAndSetIfChanged(ref _parent2CitizenshipStatus, value);
    }

    // Criminal History/Background
    private bool _hasCriminalRecord = false;
    public bool HasCriminalRecord
    {
        get => _hasCriminalRecord;
        set => this.RaiseAndSetIfChanged(ref _hasCriminalRecord, value);
    }

    private string _criminalRecordDetails = string.Empty;
    public string CriminalRecordDetails
    {
        get => _criminalRecordDetails;
        set => this.RaiseAndSetIfChanged(ref _criminalRecordDetails, value);
    }

    // Additional Identification
    private string _passportNumber = string.Empty;
    public string PassportNumber
    {
        get => _passportNumber;
        set => this.RaiseAndSetIfChanged(ref _passportNumber, value);
    }

    private string _passportIssuingCountry = string.Empty;
    public string PassportIssuingCountry
    {
        get => _passportIssuingCountry;
        set => this.RaiseAndSetIfChanged(ref _passportIssuingCountry, value);
    }

    private DateTime? _passportExpiryDate;
    public DateTime? PassportExpiryDate
    {
        get => _passportExpiryDate;
        set => this.RaiseAndSetIfChanged(ref _passportExpiryDate, value);
    }

    private string _nationalIdNumber = string.Empty;
    public string NationalIdNumber
    {
        get => _nationalIdNumber;
        set => this.RaiseAndSetIfChanged(ref _nationalIdNumber, value);
    }

    private string _nationalIdIssuingCountry = string.Empty;
    public string NationalIdIssuingCountry
    {
        get => _nationalIdIssuingCountry;
        set => this.RaiseAndSetIfChanged(ref _nationalIdIssuingCountry, value);
    }

    // Previous Address History
    private string _previousAddress1 = string.Empty;
    public string PreviousAddress1
    {
        get => _previousAddress1;
        set => this.RaiseAndSetIfChanged(ref _previousAddress1, value);
    }

    private string _previousAddress1Dates = string.Empty;
    public string PreviousAddress1Dates
    {
        get => _previousAddress1Dates;
        set => this.RaiseAndSetIfChanged(ref _previousAddress1Dates, value);
    }

    private string _previousAddress2 = string.Empty;
    public string PreviousAddress2
    {
        get => _previousAddress2;
        set => this.RaiseAndSetIfChanged(ref _previousAddress2, value);
    }

    private string _previousAddress2Dates = string.Empty;
    public string PreviousAddress2Dates
    {
        get => _previousAddress2Dates;
        set => this.RaiseAndSetIfChanged(ref _previousAddress2Dates, value);
    }

    // References
    private string _reference1Name = string.Empty;
    public string Reference1Name
    {
        get => _reference1Name;
        set => this.RaiseAndSetIfChanged(ref _reference1Name, value);
    }

    private string _reference1Relationship = string.Empty;
    public string Reference1Relationship
    {
        get => _reference1Relationship;
        set => this.RaiseAndSetIfChanged(ref _reference1Relationship, value);
    }

    private string _reference1Phone = string.Empty;
    public string Reference1Phone
    {
        get => _reference1Phone;
        set => this.RaiseAndSetIfChanged(ref _reference1Phone, value);
    }

    private string _reference1Address = string.Empty;
    public string Reference1Address
    {
        get => _reference1Address;
        set => this.RaiseAndSetIfChanged(ref _reference1Address, value);
    }

    private string _reference2Name = string.Empty;
    public string Reference2Name
    {
        get => _reference2Name;
        set => this.RaiseAndSetIfChanged(ref _reference2Name, value);
    }

    private string _reference2Relationship = string.Empty;
    public string Reference2Relationship
    {
        get => _reference2Relationship;
        set => this.RaiseAndSetIfChanged(ref _reference2Relationship, value);
    }

    private string _reference2Phone = string.Empty;
    public string Reference2Phone
    {
        get => _reference2Phone;
        set => this.RaiseAndSetIfChanged(ref _reference2Phone, value);
    }

    private string _reference2Address = string.Empty;
    public string Reference2Address
    {
        get => _reference2Address;
        set => this.RaiseAndSetIfChanged(ref _reference2Address, value);
    }

    // Travel History
    private bool _hasTraveledOutsideCountry = false;
    public bool HasTraveledOutsideCountry
    {
        get => _hasTraveledOutsideCountry;
        set => this.RaiseAndSetIfChanged(ref _hasTraveledOutsideCountry, value);
    }

    private string _travelHistoryDetails = string.Empty;
    public string TravelHistoryDetails
    {
        get => _travelHistoryDetails;
        set => this.RaiseAndSetIfChanged(ref _travelHistoryDetails, value);
    }

    private bool _submitted;
    public bool Submitted
    {
        get => _submitted;
        private set => this.RaiseAndSetIfChanged(ref _submitted, value);
    }

    public bool CanSubmit =>
        !string.IsNullOrWhiteSpace(FirstName) &&
        !string.IsNullOrWhiteSpace(LastName) &&
        DateOfBirth.HasValue &&
        !string.IsNullOrWhiteSpace(Email) &&
        PersonalDocuments is { Count: > 0 } &&
        PassportPhoto is { Count: > 0 } &&
        (UseSignaturePad ? !string.IsNullOrWhiteSpace(SignatureDataUrl) : SignatureImage is { Count: > 0 }) &&
        AcknowledgeTreaty &&
        SwearAllegiance &&
        AuthorizeBiometricEnrollment &&
        DeclareUnderstanding &&
        ConsentToVerification &&
        ConsentToDataProcessing;

    public ReactiveCommand<Unit, Unit> Submit { get; }

    public BecomeCitizenViewModel()
    {
        // Command is used only to flip Submitted=true after a successful server-side submit.
        // Button enablement is handled via Vm.CanSubmit in the Razor page.
        Submit = ReactiveCommand.Create(() => { Submitted = true; });
    }
}


