namespace NET10.Core.Interfaces;

// ═══════════════════════════════════════════════════════════════════════════════
// IERAHKWA HOSPITAL RECORDS MANAGEMENT SYSTEM
// Complete Patient & Medical Records Management
// ═══════════════════════════════════════════════════════════════════════════════

public interface IHospitalRoomService
{
    Task<List<HospitalRoom>> GetAllAsync();
    Task<HospitalRoom?> GetByIdAsync(Guid id);
    Task<List<HospitalRoom>> GetByTypeAsync(HospitalRoomType type);
    Task<List<HospitalRoom>> GetAvailableAsync();
    Task<HospitalRoom> CreateAsync(HospitalRoom room);
    Task<HospitalRoom> UpdateAsync(HospitalRoom room);
    Task<bool> SetStatusAsync(Guid id, HospitalRoomStatus status);
    Task<bool> DeleteAsync(Guid id);
    Task<RoomOccupancyReport> GetOccupancyReportAsync();
}

public interface IPhysicianService
{
    Task<List<Physician>> GetAllAsync();
    Task<Physician?> GetByIdAsync(Guid id);
    Task<Physician?> GetByLicenseNumberAsync(string licenseNumber);
    Task<List<Physician>> GetByDepartmentAsync(string department);
    Task<List<Physician>> GetBySpecializationAsync(string specialization);
    Task<Physician> CreateAsync(Physician physician);
    Task<Physician> UpdateAsync(Physician physician);
    Task<bool> SetStatusAsync(Guid id, PhysicianStatus status);
    Task<List<Patient>> GetPatientsAsync(Guid physicianId);
    Task<PhysicianSchedule> GetScheduleAsync(Guid physicianId, DateTime date);
}

public interface IDiagnosisService
{
    Task<List<Diagnosis>> GetAllAsync();
    Task<Diagnosis?> GetByIdAsync(Guid id);
    Task<List<Diagnosis>> GetByCodeAsync(string code);
    Task<List<Diagnosis>> GetByCategoryAsync(string category);
    Task<Diagnosis> CreateAsync(Diagnosis diagnosis);
    Task<Diagnosis> UpdateAsync(Diagnosis diagnosis);
    Task<List<Diagnosis>> SearchAsync(string searchTerm);
}

public interface IPatientService
{
    Task<List<Patient>> GetAllAsync();
    Task<Patient?> GetByIdAsync(Guid id);
    Task<Patient?> GetByMRNAsync(string medicalRecordNumber);
    Task<List<Patient>> GetAdmittedAsync();
    Task<List<Patient>> GetByPhysicianAsync(Guid physicianId);
    Task<Patient> RegisterAsync(Patient patient);
    Task<Patient> UpdateAsync(Patient patient);
    Task<bool> DeleteAsync(Guid id);
    Task<List<PatientAdmission>> GetAdmissionHistoryAsync(Guid patientId);
    Task<List<DiagnosticTest>> GetDiagnosticTestsAsync(Guid patientId);
    Task<PatientMedicalRecord> GetMedicalRecordAsync(Guid patientId);
}

public interface IAdmissionService
{
    Task<PatientAdmission> AdmitPatientAsync(AdmitPatientRequest request);
    Task<PatientAdmission> DischargePatientAsync(Guid admissionId, DischargeRequest request);
    Task<PatientAdmission?> GetAdmissionAsync(Guid id);
    Task<PatientAdmission?> GetCurrentAdmissionAsync(Guid patientId);
    Task<List<PatientAdmission>> GetActiveAdmissionsAsync();
    Task<List<PatientAdmission>> GetAdmissionsByDateAsync(DateTime from, DateTime to);
    Task<PatientAdmission> TransferPatientAsync(Guid admissionId, Guid newRoomId, string reason);
    Task<PatientAdmission> UpdateAsync(PatientAdmission admission);
}

public interface IDiagnosticTestService
{
    Task<DiagnosticTest> OrderTestAsync(OrderTestRequest request);
    Task<DiagnosticTest> RecordResultAsync(Guid testId, TestResultRequest result);
    Task<DiagnosticTest?> GetTestAsync(Guid id);
    Task<List<DiagnosticTest>> GetPatientTestsAsync(Guid patientId);
    Task<List<DiagnosticTest>> GetPendingTestsAsync();
    Task<List<DiagnosticTest>> GetTestsByDateAsync(DateTime date);
    Task<List<TestType>> GetTestTypesAsync();
    Task<TestType> CreateTestTypeAsync(TestType testType);
}

public interface IHospitalReportService
{
    Task<HospitalDashboard> GetDashboardAsync();
    Task<AdmissionReport> GetAdmissionReportAsync(DateTime from, DateTime to);
    Task<DischargeReport> GetDischargeReportAsync(DateTime from, DateTime to);
    Task<DiagnosticReport> GetDiagnosticReportAsync(DateTime from, DateTime to);
    Task<HospitalOccupancyReport> GetOccupancyReportAsync(DateTime date);
    Task<PhysicianReport> GetPhysicianReportAsync(DateTime from, DateTime to);
    Task<PatientStatisticsReport> GetPatientStatisticsAsync(DateTime from, DateTime to);
}

// ═══════════════════════════════════════════════════════════════════════════════
// HOSPITAL MODELS
// ═══════════════════════════════════════════════════════════════════════════════

public class HospitalRoom
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string RoomNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public HospitalRoomType Type { get; set; } = HospitalRoomType.General;
    public HospitalRoomStatus Status { get; set; } = HospitalRoomStatus.Available;
    public string Floor { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
    public int BedCount { get; set; } = 1;
    public int OccupiedBeds { get; set; }
    public int AvailableBeds => BedCount - OccupiedBeds;
    public decimal DailyRate { get; set; }
    public string? Description { get; set; }
    public string? Equipment { get; set; }
    public Guid? CurrentPatientId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum HospitalRoomType
{
    General,
    Private,
    SemiPrivate,
    ICU,
    NICU,
    CCU,
    Emergency,
    Operating,
    Recovery,
    Maternity,
    Pediatric,
    Isolation
}

public enum HospitalRoomStatus
{
    Available,
    Occupied,
    Reserved,
    Maintenance,
    Cleaning,
    OutOfService
}

public class Physician
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string EmployeeId { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"Dr. {FirstName} {LastName}";
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public PhysicianRank Rank { get; set; } = PhysicianRank.Resident;
    public PhysicianStatus Status { get; set; } = PhysicianStatus.Active;
    public DateTime HireDate { get; set; }
    public string? Education { get; set; }
    public string? Certifications { get; set; }
    public string? Office { get; set; }
    public string? Schedule { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum PhysicianRank
{
    Intern,
    Resident,
    Fellow,
    Attending,
    Consultant,
    ChiefOfMedicine,
    DepartmentHead,
    MedicalDirector
}

public enum PhysicianStatus
{
    Active,
    OnLeave,
    Vacation,
    Training,
    Suspended,
    Retired,
    Inactive
}

public class PhysicianSchedule
{
    public Guid PhysicianId { get; set; }
    public DateTime Date { get; set; }
    public List<ScheduleSlot> Slots { get; set; } = new();
    public int TotalAppointments { get; set; }
    public int CompletedAppointments { get; set; }
}

public class ScheduleSlot
{
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Type { get; set; } = string.Empty;
    public Guid? PatientId { get; set; }
    public string? PatientName { get; set; }
    public bool IsAvailable { get; set; }
}

public class Diagnosis
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DiagnosisSeverity Severity { get; set; } = DiagnosisSeverity.Moderate;
    public string? Symptoms { get; set; }
    public string? TreatmentGuidelines { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum DiagnosisSeverity
{
    Minor,
    Moderate,
    Serious,
    Severe,
    Critical,
    LifeThreatening
}

public class Patient
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string MRN { get; set; } = string.Empty; // Medical Record Number
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public DateTime DateOfBirth { get; set; }
    public int Age => CalculateAge();
    public string Gender { get; set; } = string.Empty;
    public string BloodType { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? InsuranceProvider { get; set; }
    public string? InsurancePolicyNumber { get; set; }
    public string? Allergies { get; set; }
    public string? ChronicConditions { get; set; }
    public string? CurrentMedications { get; set; }
    public PatientStatus Status { get; set; } = PatientStatus.Registered;
    public Guid? PrimaryPhysicianId { get; set; }
    public string? PrimaryPhysicianName { get; set; }
    public Guid? CurrentAdmissionId { get; set; }
    public string? CurrentRoomNumber { get; set; }
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastVisit { get; set; }
    
    private int CalculateAge()
    {
        var today = DateTime.Today;
        var age = today.Year - DateOfBirth.Year;
        if (DateOfBirth.Date > today.AddYears(-age)) age--;
        return age;
    }
}

public enum PatientStatus
{
    Registered,
    Admitted,
    Discharged,
    OutPatient,
    Emergency,
    Deceased,
    Transferred
}

public class PatientAdmission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string AdmissionNumber { get; set; } = string.Empty;
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string MRN { get; set; } = string.Empty;
    public Guid RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public Guid PhysicianId { get; set; }
    public string PhysicianName { get; set; } = string.Empty;
    public DateTime AdmissionDate { get; set; }
    public DateTime? DischargeDate { get; set; }
    public string AdmissionType { get; set; } = string.Empty;
    public string? AdmissionReason { get; set; }
    public Guid? PrimaryDiagnosisId { get; set; }
    public string? PrimaryDiagnosis { get; set; }
    public List<string> SecondaryDiagnoses { get; set; } = new();
    public AdmissionStatus Status { get; set; } = AdmissionStatus.Active;
    public string? DischargeNotes { get; set; }
    public string? DischargeSummary { get; set; }
    public string? FollowUpInstructions { get; set; }
    public decimal TotalCharges { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal Balance => TotalCharges - AmountPaid;
    public List<AdmissionNote> Notes { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum AdmissionStatus
{
    Active,
    Transferred,
    Discharged,
    Deceased,
    AMA // Against Medical Advice
}

public class AdmissionNote
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Note { get; set; } = string.Empty;
    public string NoteType { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class AdmitPatientRequest
{
    public Guid PatientId { get; set; }
    public Guid RoomId { get; set; }
    public Guid PhysicianId { get; set; }
    public string AdmissionType { get; set; } = "Elective";
    public string? AdmissionReason { get; set; }
    public Guid? PrimaryDiagnosisId { get; set; }
    public string? Notes { get; set; }
}

public class DischargeRequest
{
    public string DischargeSummary { get; set; } = string.Empty;
    public string? DischargeNotes { get; set; }
    public string? FollowUpInstructions { get; set; }
    public List<string>? Prescriptions { get; set; }
    public DateTime? FollowUpDate { get; set; }
}

public class DiagnosticTest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TestNumber { get; set; } = string.Empty;
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string MRN { get; set; } = string.Empty;
    public Guid? AdmissionId { get; set; }
    public Guid TestTypeId { get; set; }
    public string TestName { get; set; } = string.Empty;
    public string TestCategory { get; set; } = string.Empty;
    public Guid OrderedById { get; set; }
    public string OrderedByName { get; set; } = string.Empty;
    public DateTime OrderedAt { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public DateTime? PerformedAt { get; set; }
    public DateTime? ResultsAt { get; set; }
    public TestStatus Status { get; set; } = TestStatus.Ordered;
    public string? Results { get; set; }
    public string? Interpretation { get; set; }
    public ResultFlag? ResultFlag { get; set; }
    public string? Notes { get; set; }
    public Guid? PerformedById { get; set; }
    public string? PerformedByName { get; set; }
    public decimal Cost { get; set; }
}

public enum TestStatus
{
    Ordered,
    Scheduled,
    InProgress,
    Completed,
    Cancelled,
    OnHold
}

public enum ResultFlag
{
    Normal,
    Abnormal,
    Critical,
    Inconclusive,
    Pending
}

public class TestType
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? NormalRange { get; set; }
    public string? Unit { get; set; }
    public decimal Cost { get; set; }
    public int TurnaroundTimeHours { get; set; }
    public string? PreparationInstructions { get; set; }
    public bool IsActive { get; set; } = true;
}

public class OrderTestRequest
{
    public Guid PatientId { get; set; }
    public Guid? AdmissionId { get; set; }
    public Guid TestTypeId { get; set; }
    public Guid OrderedById { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public string? Notes { get; set; }
    public bool IsUrgent { get; set; }
}

public class TestResultRequest
{
    public string Results { get; set; } = string.Empty;
    public string? Interpretation { get; set; }
    public ResultFlag ResultFlag { get; set; } = ResultFlag.Normal;
    public Guid PerformedById { get; set; }
    public string? Notes { get; set; }
}

public class PatientMedicalRecord
{
    public Guid PatientId { get; set; }
    public string MRN { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public List<PatientAdmission> Admissions { get; set; } = new();
    public List<DiagnosticTest> Tests { get; set; } = new();
    public List<string> Diagnoses { get; set; } = new();
    public string? Allergies { get; set; }
    public string? ChronicConditions { get; set; }
    public string? CurrentMedications { get; set; }
    public int TotalVisits { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

// ═══════════════════════════════════════════════════════════════════════════════
// HOSPITAL REPORT MODELS
// ═══════════════════════════════════════════════════════════════════════════════

public class HospitalDashboard
{
    public int TotalRooms { get; set; }
    public int AvailableRooms { get; set; }
    public int OccupiedRooms { get; set; }
    public int TotalPatients { get; set; }
    public int AdmittedPatients { get; set; }
    public int TodayAdmissions { get; set; }
    public int TodayDischarges { get; set; }
    public int PendingTests { get; set; }
    public int ActivePhysicians { get; set; }
    public decimal OccupancyRate { get; set; }
    public List<PatientAdmission> RecentAdmissions { get; set; } = new();
    public List<PatientAdmission> PendingDischarges { get; set; } = new();
    public Dictionary<HospitalRoomType, int> RoomsByType { get; set; } = new();
    public Dictionary<string, int> PatientsByDepartment { get; set; } = new();
}

public class AdmissionReport
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalAdmissions { get; set; }
    public int EmergencyAdmissions { get; set; }
    public int ElectiveAdmissions { get; set; }
    public decimal AverageLengthOfStay { get; set; }
    public Dictionary<string, int> ByDepartment { get; set; } = new();
    public Dictionary<string, int> ByDiagnosis { get; set; } = new();
    public Dictionary<string, int> ByDay { get; set; } = new();
    public List<PatientAdmission> Admissions { get; set; } = new();
}

public class DischargeReport
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalDischarges { get; set; }
    public int RegularDischarges { get; set; }
    public int TransferredPatients { get; set; }
    public int DeathCount { get; set; }
    public int AMACount { get; set; }
    public decimal AverageLengthOfStay { get; set; }
    public Dictionary<string, int> ByDepartment { get; set; } = new();
    public List<PatientAdmission> Discharges { get; set; } = new();
}

public class DiagnosticReport
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalTests { get; set; }
    public int CompletedTests { get; set; }
    public int PendingTests { get; set; }
    public int AbnormalResults { get; set; }
    public int CriticalResults { get; set; }
    public Dictionary<string, int> ByCategory { get; set; } = new();
    public Dictionary<string, int> ByDay { get; set; } = new();
    public decimal TotalRevenue { get; set; }
}

public class HospitalOccupancyReport
{
    public DateTime Date { get; set; }
    public int TotalBeds { get; set; }
    public int OccupiedBeds { get; set; }
    public int AvailableBeds { get; set; }
    public decimal OccupancyRate { get; set; }
    public Dictionary<HospitalRoomType, OccupancyByType> ByRoomType { get; set; } = new();
}

public class OccupancyByType
{
    public int TotalBeds { get; set; }
    public int OccupiedBeds { get; set; }
    public decimal OccupancyRate { get; set; }
}

public class RoomOccupancyReport
{
    public int TotalRooms { get; set; }
    public int AvailableRooms { get; set; }
    public int OccupiedRooms { get; set; }
    public decimal OccupancyRate { get; set; }
    public List<HospitalRoom> Rooms { get; set; } = new();
}

public class PhysicianReport
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalPhysicians { get; set; }
    public int ActivePhysicians { get; set; }
    public Dictionary<string, int> ByDepartment { get; set; } = new();
    public Dictionary<string, int> BySpecialization { get; set; } = new();
    public List<PhysicianStats> TopPhysicians { get; set; } = new();
}

public class PhysicianStats
{
    public Guid PhysicianId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public int PatientCount { get; set; }
    public int AdmissionCount { get; set; }
    public int DischargeCount { get; set; }
}

public class PatientStatisticsReport
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalPatients { get; set; }
    public int NewPatients { get; set; }
    public int ReturningPatients { get; set; }
    public Dictionary<string, int> ByGender { get; set; } = new();
    public Dictionary<string, int> ByAgeGroup { get; set; } = new();
    public Dictionary<string, int> ByBloodType { get; set; } = new();
}
