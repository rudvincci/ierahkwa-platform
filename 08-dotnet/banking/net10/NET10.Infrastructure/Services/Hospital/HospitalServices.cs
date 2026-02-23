using NET10.Core.Interfaces;

namespace NET10.Infrastructure.Services.Hospital;

// ═══════════════════════════════════════════════════════════════════════════════
// IERAHKWA HOSPITAL RECORDS MANAGEMENT SYSTEM
// Complete Medical Records & Patient Management
// ═══════════════════════════════════════════════════════════════════════════════

public class HospitalRoomService : IHospitalRoomService
{
    private static readonly List<HospitalRoom> _rooms = InitializeDemoRooms();
    
    private static List<HospitalRoom> InitializeDemoRooms()
    {
        return new List<HospitalRoom>
        {
            // General Rooms
            new() { RoomNumber = "101", Name = "General Ward A", Type = HospitalRoomType.General, Floor = "1", Building = "Main", BedCount = 4, DailyRate = 150, Status = HospitalRoomStatus.Available },
            new() { RoomNumber = "102", Name = "General Ward B", Type = HospitalRoomType.General, Floor = "1", Building = "Main", BedCount = 4, DailyRate = 150, Status = HospitalRoomStatus.Available },
            new() { RoomNumber = "103", Name = "General Ward C", Type = HospitalRoomType.General, Floor = "1", Building = "Main", BedCount = 4, DailyRate = 150, Status = HospitalRoomStatus.Available },
            // Private Rooms
            new() { RoomNumber = "201", Name = "Private Room 1", Type = HospitalRoomType.Private, Floor = "2", Building = "Main", BedCount = 1, DailyRate = 450, Status = HospitalRoomStatus.Available },
            new() { RoomNumber = "202", Name = "Private Room 2", Type = HospitalRoomType.Private, Floor = "2", Building = "Main", BedCount = 1, DailyRate = 450, Status = HospitalRoomStatus.Available },
            new() { RoomNumber = "203", Name = "Private Room 3", Type = HospitalRoomType.Private, Floor = "2", Building = "Main", BedCount = 1, DailyRate = 450, Status = HospitalRoomStatus.Available },
            // Semi-Private
            new() { RoomNumber = "204", Name = "Semi-Private 1", Type = HospitalRoomType.SemiPrivate, Floor = "2", Building = "Main", BedCount = 2, DailyRate = 300, Status = HospitalRoomStatus.Available },
            new() { RoomNumber = "205", Name = "Semi-Private 2", Type = HospitalRoomType.SemiPrivate, Floor = "2", Building = "Main", BedCount = 2, DailyRate = 300, Status = HospitalRoomStatus.Available },
            // ICU
            new() { RoomNumber = "ICU-01", Name = "ICU Bay 1", Type = HospitalRoomType.ICU, Floor = "3", Building = "Critical Care", BedCount = 1, DailyRate = 1200, Status = HospitalRoomStatus.Available, Equipment = "Ventilator, Monitor, Defibrillator" },
            new() { RoomNumber = "ICU-02", Name = "ICU Bay 2", Type = HospitalRoomType.ICU, Floor = "3", Building = "Critical Care", BedCount = 1, DailyRate = 1200, Status = HospitalRoomStatus.Available, Equipment = "Ventilator, Monitor, Defibrillator" },
            new() { RoomNumber = "ICU-03", Name = "ICU Bay 3", Type = HospitalRoomType.ICU, Floor = "3", Building = "Critical Care", BedCount = 1, DailyRate = 1200, Status = HospitalRoomStatus.Available, Equipment = "Ventilator, Monitor, Defibrillator" },
            // Emergency
            new() { RoomNumber = "ER-01", Name = "Emergency Bay 1", Type = HospitalRoomType.Emergency, Floor = "G", Building = "Emergency", BedCount = 1, DailyRate = 500, Status = HospitalRoomStatus.Available },
            new() { RoomNumber = "ER-02", Name = "Emergency Bay 2", Type = HospitalRoomType.Emergency, Floor = "G", Building = "Emergency", BedCount = 1, DailyRate = 500, Status = HospitalRoomStatus.Available },
            new() { RoomNumber = "ER-03", Name = "Emergency Bay 3", Type = HospitalRoomType.Emergency, Floor = "G", Building = "Emergency", BedCount = 1, DailyRate = 500, Status = HospitalRoomStatus.Available },
            // Operating Rooms
            new() { RoomNumber = "OR-01", Name = "Operating Room 1", Type = HospitalRoomType.Operating, Floor = "4", Building = "Surgical", BedCount = 1, DailyRate = 2500, Status = HospitalRoomStatus.Available },
            new() { RoomNumber = "OR-02", Name = "Operating Room 2", Type = HospitalRoomType.Operating, Floor = "4", Building = "Surgical", BedCount = 1, DailyRate = 2500, Status = HospitalRoomStatus.Available },
            // Maternity
            new() { RoomNumber = "MAT-01", Name = "Maternity Suite 1", Type = HospitalRoomType.Maternity, Floor = "5", Building = "Women's Health", BedCount = 1, DailyRate = 600, Status = HospitalRoomStatus.Available },
            new() { RoomNumber = "MAT-02", Name = "Maternity Suite 2", Type = HospitalRoomType.Maternity, Floor = "5", Building = "Women's Health", BedCount = 1, DailyRate = 600, Status = HospitalRoomStatus.Available },
            // Pediatric
            new() { RoomNumber = "PED-01", Name = "Pediatric Room 1", Type = HospitalRoomType.Pediatric, Floor = "6", Building = "Children's", BedCount = 2, DailyRate = 350, Status = HospitalRoomStatus.Available },
            new() { RoomNumber = "PED-02", Name = "Pediatric Room 2", Type = HospitalRoomType.Pediatric, Floor = "6", Building = "Children's", BedCount = 2, DailyRate = 350, Status = HospitalRoomStatus.Available }
        };
    }
    
    public Task<List<HospitalRoom>> GetAllAsync() => Task.FromResult(_rooms.Where(r => r.IsActive).ToList());
    
    public Task<HospitalRoom?> GetByIdAsync(Guid id) => Task.FromResult(_rooms.FirstOrDefault(r => r.Id == id));
    
    public Task<List<HospitalRoom>> GetByTypeAsync(HospitalRoomType type) => 
        Task.FromResult(_rooms.Where(r => r.Type == type && r.IsActive).ToList());
    
    public Task<List<HospitalRoom>> GetAvailableAsync() => 
        Task.FromResult(_rooms.Where(r => r.Status == HospitalRoomStatus.Available && r.IsActive).ToList());
    
    public Task<HospitalRoom> CreateAsync(HospitalRoom room)
    {
        room.Id = Guid.NewGuid();
        room.CreatedAt = DateTime.UtcNow;
        _rooms.Add(room);
        return Task.FromResult(room);
    }
    
    public Task<HospitalRoom> UpdateAsync(HospitalRoom room)
    {
        var index = _rooms.FindIndex(r => r.Id == room.Id);
        if (index >= 0) _rooms[index] = room;
        return Task.FromResult(room);
    }
    
    public Task<bool> SetStatusAsync(Guid id, HospitalRoomStatus status)
    {
        var room = _rooms.FirstOrDefault(r => r.Id == id);
        if (room != null) { room.Status = status; return Task.FromResult(true); }
        return Task.FromResult(false);
    }
    
    public Task<bool> DeleteAsync(Guid id)
    {
        var room = _rooms.FirstOrDefault(r => r.Id == id);
        if (room != null) { room.IsActive = false; return Task.FromResult(true); }
        return Task.FromResult(false);
    }
    
    public Task<RoomOccupancyReport> GetOccupancyReportAsync()
    {
        var activeRooms = _rooms.Where(r => r.IsActive).ToList();
        var occupied = activeRooms.Count(r => r.Status == HospitalRoomStatus.Occupied);
        return Task.FromResult(new RoomOccupancyReport
        {
            TotalRooms = activeRooms.Count,
            AvailableRooms = activeRooms.Count(r => r.Status == HospitalRoomStatus.Available),
            OccupiedRooms = occupied,
            OccupancyRate = activeRooms.Count > 0 ? (occupied * 100m / activeRooms.Count) : 0,
            Rooms = activeRooms
        });
    }
}

public class PhysicianService : IPhysicianService
{
    private static readonly List<Physician> _physicians = InitializeDemoPhysicians();
    
    private static List<Physician> InitializeDemoPhysicians()
    {
        return new List<Physician>
        {
            new() { EmployeeId = "PHY-001", LicenseNumber = "MD-12345", FirstName = "Karakwine", LastName = "Hill", Email = "k.hill@ierahkwa-health.org", Phone = "+1-777-555-2001", Department = "Internal Medicine", Specialization = "Cardiology", Rank = PhysicianRank.Attending, HireDate = new DateTime(2018, 3, 1) },
            new() { EmployeeId = "PHY-002", LicenseNumber = "MD-12346", FirstName = "Sarah", LastName = "Redbird", Email = "s.redbird@ierahkwa-health.org", Phone = "+1-777-555-2002", Department = "Surgery", Specialization = "General Surgery", Rank = PhysicianRank.Consultant, HireDate = new DateTime(2015, 6, 15) },
            new() { EmployeeId = "PHY-003", LicenseNumber = "MD-12347", FirstName = "Michael", LastName = "Running Deer", Email = "m.runningdeer@ierahkwa-health.org", Phone = "+1-777-555-2003", Department = "Pediatrics", Specialization = "Pediatric Medicine", Rank = PhysicianRank.Attending, HireDate = new DateTime(2019, 1, 10) },
            new() { EmployeeId = "PHY-004", LicenseNumber = "MD-12348", FirstName = "Elena", LastName = "Thompson", Email = "e.thompson@ierahkwa-health.org", Phone = "+1-777-555-2004", Department = "Emergency", Specialization = "Emergency Medicine", Rank = PhysicianRank.Fellow, HireDate = new DateTime(2021, 8, 1) },
            new() { EmployeeId = "PHY-005", LicenseNumber = "MD-12349", FirstName = "James", LastName = "White Eagle", Email = "j.whiteeagle@ierahkwa-health.org", Phone = "+1-777-555-2005", Department = "Orthopedics", Specialization = "Orthopedic Surgery", Rank = PhysicianRank.Consultant, HireDate = new DateTime(2016, 4, 20) },
            new() { EmployeeId = "PHY-006", LicenseNumber = "MD-12350", FirstName = "Maria", LastName = "Garcia", Email = "m.garcia@ierahkwa-health.org", Phone = "+1-777-555-2006", Department = "OB/GYN", Specialization = "Obstetrics & Gynecology", Rank = PhysicianRank.Attending, HireDate = new DateTime(2017, 9, 5) },
            new() { EmployeeId = "PHY-007", LicenseNumber = "MD-12351", FirstName = "David", LastName = "Oaks", Email = "d.oaks@ierahkwa-health.org", Phone = "+1-777-555-2007", Department = "Neurology", Specialization = "Neurology", Rank = PhysicianRank.DepartmentHead, HireDate = new DateTime(2012, 2, 14) },
            new() { EmployeeId = "PHY-008", LicenseNumber = "MD-12352", FirstName = "Linda", LastName = "Sky", Email = "l.sky@ierahkwa-health.org", Phone = "+1-777-555-2008", Department = "Radiology", Specialization = "Diagnostic Radiology", Rank = PhysicianRank.Attending, HireDate = new DateTime(2020, 5, 1) }
        };
    }
    
    public Task<List<Physician>> GetAllAsync() => Task.FromResult(_physicians.Where(p => p.IsActive).ToList());
    
    public Task<Physician?> GetByIdAsync(Guid id) => Task.FromResult(_physicians.FirstOrDefault(p => p.Id == id));
    
    public Task<Physician?> GetByLicenseNumberAsync(string licenseNumber) => 
        Task.FromResult(_physicians.FirstOrDefault(p => p.LicenseNumber == licenseNumber));
    
    public Task<List<Physician>> GetByDepartmentAsync(string department) => 
        Task.FromResult(_physicians.Where(p => p.Department.Equals(department, StringComparison.OrdinalIgnoreCase) && p.IsActive).ToList());
    
    public Task<List<Physician>> GetBySpecializationAsync(string specialization) => 
        Task.FromResult(_physicians.Where(p => p.Specialization.Contains(specialization, StringComparison.OrdinalIgnoreCase) && p.IsActive).ToList());
    
    public Task<Physician> CreateAsync(Physician physician)
    {
        physician.Id = Guid.NewGuid();
        physician.CreatedAt = DateTime.UtcNow;
        _physicians.Add(physician);
        return Task.FromResult(physician);
    }
    
    public Task<Physician> UpdateAsync(Physician physician)
    {
        var index = _physicians.FindIndex(p => p.Id == physician.Id);
        if (index >= 0) _physicians[index] = physician;
        return Task.FromResult(physician);
    }
    
    public Task<bool> SetStatusAsync(Guid id, PhysicianStatus status)
    {
        var physician = _physicians.FirstOrDefault(p => p.Id == id);
        if (physician != null) { physician.Status = status; return Task.FromResult(true); }
        return Task.FromResult(false);
    }
    
    public Task<List<Patient>> GetPatientsAsync(Guid physicianId) => Task.FromResult(new List<Patient>());
    
    public Task<PhysicianSchedule> GetScheduleAsync(Guid physicianId, DateTime date) =>
        Task.FromResult(new PhysicianSchedule { PhysicianId = physicianId, Date = date, TotalAppointments = 12, CompletedAppointments = 5 });
}

public class DiagnosisService : IDiagnosisService
{
    private static readonly List<Diagnosis> _diagnoses = InitializeDemoDiagnoses();
    
    private static List<Diagnosis> InitializeDemoDiagnoses()
    {
        return new List<Diagnosis>
        {
            new() { Code = "J06.9", Name = "Acute Upper Respiratory Infection", Category = "Respiratory", Severity = DiagnosisSeverity.Minor },
            new() { Code = "I10", Name = "Essential Hypertension", Category = "Cardiovascular", Severity = DiagnosisSeverity.Moderate },
            new() { Code = "E11.9", Name = "Type 2 Diabetes Mellitus", Category = "Endocrine", Severity = DiagnosisSeverity.Moderate },
            new() { Code = "J18.9", Name = "Pneumonia, Unspecified", Category = "Respiratory", Severity = DiagnosisSeverity.Serious },
            new() { Code = "I21.9", Name = "Acute Myocardial Infarction", Category = "Cardiovascular", Severity = DiagnosisSeverity.Critical },
            new() { Code = "K35.80", Name = "Acute Appendicitis", Category = "Gastrointestinal", Severity = DiagnosisSeverity.Serious },
            new() { Code = "S72.001A", Name = "Fracture of Femur", Category = "Musculoskeletal", Severity = DiagnosisSeverity.Serious },
            new() { Code = "O80", Name = "Normal Delivery", Category = "Obstetrics", Severity = DiagnosisSeverity.Minor },
            new() { Code = "G40.909", Name = "Epilepsy, Unspecified", Category = "Neurological", Severity = DiagnosisSeverity.Moderate },
            new() { Code = "N39.0", Name = "Urinary Tract Infection", Category = "Genitourinary", Severity = DiagnosisSeverity.Minor },
            new() { Code = "A09", Name = "Gastroenteritis", Category = "Gastrointestinal", Severity = DiagnosisSeverity.Minor },
            new() { Code = "J45.909", Name = "Asthma, Unspecified", Category = "Respiratory", Severity = DiagnosisSeverity.Moderate }
        };
    }
    
    public Task<List<Diagnosis>> GetAllAsync() => Task.FromResult(_diagnoses.Where(d => d.IsActive).ToList());
    
    public Task<Diagnosis?> GetByIdAsync(Guid id) => Task.FromResult(_diagnoses.FirstOrDefault(d => d.Id == id));
    
    public Task<List<Diagnosis>> GetByCodeAsync(string code) => 
        Task.FromResult(_diagnoses.Where(d => d.Code.StartsWith(code, StringComparison.OrdinalIgnoreCase)).ToList());
    
    public Task<List<Diagnosis>> GetByCategoryAsync(string category) => 
        Task.FromResult(_diagnoses.Where(d => d.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList());
    
    public Task<Diagnosis> CreateAsync(Diagnosis diagnosis)
    {
        diagnosis.Id = Guid.NewGuid();
        _diagnoses.Add(diagnosis);
        return Task.FromResult(diagnosis);
    }
    
    public Task<Diagnosis> UpdateAsync(Diagnosis diagnosis)
    {
        var index = _diagnoses.FindIndex(d => d.Id == diagnosis.Id);
        if (index >= 0) _diagnoses[index] = diagnosis;
        return Task.FromResult(diagnosis);
    }
    
    public Task<List<Diagnosis>> SearchAsync(string searchTerm) =>
        Task.FromResult(_diagnoses.Where(d => 
            d.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || 
            d.Code.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList());
}

public class PatientService : IPatientService
{
    private static readonly List<Patient> _patients = InitializeDemoPatients();
    private static int _mrnCounter = 100001;
    
    private static List<Patient> InitializeDemoPatients()
    {
        return new List<Patient>
        {
            new() { MRN = "MRN-100001", FirstName = "Kawennahere", LastName = "Thompson", DateOfBirth = new DateTime(1985, 3, 15), Gender = "Female", BloodType = "O+", Phone = "+1-777-555-3001", Address = "123 Oak Street, Akwesasne", EmergencyContactName = "John Thompson", EmergencyContactPhone = "+1-777-555-3002" },
            new() { MRN = "MRN-100002", FirstName = "Tehonikonrathe", LastName = "White", DateOfBirth = new DateTime(1978, 7, 22), Gender = "Male", BloodType = "A+", Phone = "+1-777-555-3003", Address = "456 Maple Ave, Akwesasne", EmergencyContactName = "Mary White", EmergencyContactPhone = "+1-777-555-3004" },
            new() { MRN = "MRN-100003", FirstName = "Maria", LastName = "Garcia", DateOfBirth = new DateTime(1992, 11, 8), Gender = "Female", BloodType = "B+", Phone = "+1-777-555-3005", Address = "789 Pine Road, Akwesasne", EmergencyContactName = "Carlos Garcia", EmergencyContactPhone = "+1-777-555-3006" },
            new() { MRN = "MRN-100004", FirstName = "James", LastName = "Running Deer", DateOfBirth = new DateTime(1965, 5, 30), Gender = "Male", BloodType = "AB+", Phone = "+1-777-555-3007", Address = "321 Elm Street, Akwesasne", ChronicConditions = "Hypertension, Diabetes Type 2" },
            new() { MRN = "MRN-100005", FirstName = "Sarah", LastName = "Sky", DateOfBirth = new DateTime(2015, 2, 14), Gender = "Female", BloodType = "O-", Phone = "+1-777-555-3008", Address = "654 Birch Lane, Akwesasne", EmergencyContactName = "Linda Sky", EmergencyContactPhone = "+1-777-555-3009" }
        };
    }
    
    public Task<List<Patient>> GetAllAsync() => Task.FromResult(_patients.ToList());
    
    public Task<Patient?> GetByIdAsync(Guid id) => Task.FromResult(_patients.FirstOrDefault(p => p.Id == id));
    
    public Task<Patient?> GetByMRNAsync(string medicalRecordNumber) => 
        Task.FromResult(_patients.FirstOrDefault(p => p.MRN == medicalRecordNumber));
    
    public Task<List<Patient>> GetAdmittedAsync() => 
        Task.FromResult(_patients.Where(p => p.Status == PatientStatus.Admitted).ToList());
    
    public Task<List<Patient>> GetByPhysicianAsync(Guid physicianId) => 
        Task.FromResult(_patients.Where(p => p.PrimaryPhysicianId == physicianId).ToList());
    
    public Task<Patient> RegisterAsync(Patient patient)
    {
        patient.Id = Guid.NewGuid();
        patient.MRN = $"MRN-{_mrnCounter++:D6}";
        patient.RegisteredAt = DateTime.UtcNow;
        _patients.Add(patient);
        return Task.FromResult(patient);
    }
    
    public Task<Patient> UpdateAsync(Patient patient)
    {
        var index = _patients.FindIndex(p => p.Id == patient.Id);
        if (index >= 0) _patients[index] = patient;
        return Task.FromResult(patient);
    }
    
    public Task<bool> DeleteAsync(Guid id)
    {
        var patient = _patients.FirstOrDefault(p => p.Id == id);
        if (patient != null) { _patients.Remove(patient); return Task.FromResult(true); }
        return Task.FromResult(false);
    }
    
    public Task<List<PatientAdmission>> GetAdmissionHistoryAsync(Guid patientId) => Task.FromResult(new List<PatientAdmission>());
    
    public Task<List<DiagnosticTest>> GetDiagnosticTestsAsync(Guid patientId) => Task.FromResult(new List<DiagnosticTest>());
    
    public async Task<PatientMedicalRecord> GetMedicalRecordAsync(Guid patientId)
    {
        var patient = await GetByIdAsync(patientId);
        return new PatientMedicalRecord
        {
            PatientId = patientId,
            MRN = patient?.MRN ?? "",
            PatientName = patient?.FullName ?? "",
            Allergies = patient?.Allergies,
            ChronicConditions = patient?.ChronicConditions,
            CurrentMedications = patient?.CurrentMedications,
            TotalVisits = 5
        };
    }
}

public class AdmissionService : IAdmissionService
{
    private static readonly List<PatientAdmission> _admissions = new();
    private static int _admissionCounter = 1001;
    private readonly IHospitalRoomService _roomService;
    private readonly IPatientService _patientService;
    private readonly IPhysicianService _physicianService;
    
    public AdmissionService(IHospitalRoomService roomService, IPatientService patientService, IPhysicianService physicianService)
    {
        _roomService = roomService;
        _patientService = patientService;
        _physicianService = physicianService;
    }
    
    public async Task<PatientAdmission> AdmitPatientAsync(AdmitPatientRequest request)
    {
        var patient = await _patientService.GetByIdAsync(request.PatientId);
        var room = await _roomService.GetByIdAsync(request.RoomId);
        var physician = await _physicianService.GetByIdAsync(request.PhysicianId);
        
        var admission = new PatientAdmission
        {
            AdmissionNumber = $"ADM-{DateTime.UtcNow:yyyyMMdd}-{_admissionCounter++:D4}",
            PatientId = request.PatientId,
            PatientName = patient?.FullName ?? "Unknown",
            MRN = patient?.MRN ?? "",
            RoomId = request.RoomId,
            RoomNumber = room?.RoomNumber ?? "",
            PhysicianId = request.PhysicianId,
            PhysicianName = physician?.FullName ?? "",
            AdmissionDate = DateTime.UtcNow,
            AdmissionType = request.AdmissionType,
            AdmissionReason = request.AdmissionReason,
            PrimaryDiagnosisId = request.PrimaryDiagnosisId,
            Status = AdmissionStatus.Active
        };
        
        if (request.Notes != null)
            admission.Notes.Add(new AdmissionNote { Note = request.Notes, NoteType = "Admission", AuthorName = physician?.FullName ?? "System" });
        
        _admissions.Add(admission);
        
        // Update room and patient status
        await _roomService.SetStatusAsync(request.RoomId, HospitalRoomStatus.Occupied);
        if (patient != null)
        {
            patient.Status = PatientStatus.Admitted;
            patient.CurrentAdmissionId = admission.Id;
            patient.CurrentRoomNumber = room?.RoomNumber;
            await _patientService.UpdateAsync(patient);
        }
        
        return admission;
    }
    
    public async Task<PatientAdmission> DischargePatientAsync(Guid admissionId, DischargeRequest request)
    {
        var admission = await GetAdmissionAsync(admissionId);
        if (admission == null) throw new InvalidOperationException("Admission not found");
        
        admission.DischargeDate = DateTime.UtcNow;
        admission.Status = AdmissionStatus.Discharged;
        admission.DischargeSummary = request.DischargeSummary;
        admission.DischargeNotes = request.DischargeNotes;
        admission.FollowUpInstructions = request.FollowUpInstructions;
        
        // Calculate charges
        var days = Math.Max(1, (int)(admission.DischargeDate.Value - admission.AdmissionDate).TotalDays);
        var room = await _roomService.GetByIdAsync(admission.RoomId);
        admission.TotalCharges = days * (room?.DailyRate ?? 150);
        
        // Free room and update patient
        await _roomService.SetStatusAsync(admission.RoomId, HospitalRoomStatus.Available);
        var patient = await _patientService.GetByIdAsync(admission.PatientId);
        if (patient != null)
        {
            patient.Status = PatientStatus.Discharged;
            patient.CurrentAdmissionId = null;
            patient.CurrentRoomNumber = null;
            patient.LastVisit = DateTime.UtcNow;
            await _patientService.UpdateAsync(patient);
        }
        
        return admission;
    }
    
    public Task<PatientAdmission?> GetAdmissionAsync(Guid id) => Task.FromResult(_admissions.FirstOrDefault(a => a.Id == id));
    
    public Task<PatientAdmission?> GetCurrentAdmissionAsync(Guid patientId) =>
        Task.FromResult(_admissions.FirstOrDefault(a => a.PatientId == patientId && a.Status == AdmissionStatus.Active));
    
    public Task<List<PatientAdmission>> GetActiveAdmissionsAsync() =>
        Task.FromResult(_admissions.Where(a => a.Status == AdmissionStatus.Active).ToList());
    
    public Task<List<PatientAdmission>> GetAdmissionsByDateAsync(DateTime from, DateTime to) =>
        Task.FromResult(_admissions.Where(a => a.AdmissionDate >= from && a.AdmissionDate <= to).ToList());
    
    public async Task<PatientAdmission> TransferPatientAsync(Guid admissionId, Guid newRoomId, string reason)
    {
        var admission = await GetAdmissionAsync(admissionId);
        if (admission == null) throw new InvalidOperationException("Admission not found");
        
        var oldRoomId = admission.RoomId;
        var newRoom = await _roomService.GetByIdAsync(newRoomId);
        
        admission.RoomId = newRoomId;
        admission.RoomNumber = newRoom?.RoomNumber ?? "";
        admission.Notes.Add(new AdmissionNote { Note = $"Transferred from room. Reason: {reason}", NoteType = "Transfer", AuthorName = "System" });
        
        await _roomService.SetStatusAsync(oldRoomId, HospitalRoomStatus.Available);
        await _roomService.SetStatusAsync(newRoomId, HospitalRoomStatus.Occupied);
        
        return admission;
    }
    
    public Task<PatientAdmission> UpdateAsync(PatientAdmission admission)
    {
        var index = _admissions.FindIndex(a => a.Id == admission.Id);
        if (index >= 0) _admissions[index] = admission;
        return Task.FromResult(admission);
    }
}

public class DiagnosticTestService : IDiagnosticTestService
{
    private static readonly List<DiagnosticTest> _tests = new();
    private static readonly List<TestType> _testTypes = InitializeTestTypes();
    private static int _testCounter = 1001;
    
    private static List<TestType> InitializeTestTypes()
    {
        return new List<TestType>
        {
            new() { Code = "CBC", Name = "Complete Blood Count", Category = "Hematology", Cost = 25, TurnaroundTimeHours = 4, NormalRange = "See ranges below" },
            new() { Code = "BMP", Name = "Basic Metabolic Panel", Category = "Chemistry", Cost = 35, TurnaroundTimeHours = 4 },
            new() { Code = "CMP", Name = "Comprehensive Metabolic Panel", Category = "Chemistry", Cost = 50, TurnaroundTimeHours = 6 },
            new() { Code = "LIP", Name = "Lipid Panel", Category = "Chemistry", Cost = 40, TurnaroundTimeHours = 6 },
            new() { Code = "UA", Name = "Urinalysis", Category = "Urinalysis", Cost = 20, TurnaroundTimeHours = 2 },
            new() { Code = "XR-CHEST", Name = "Chest X-Ray", Category = "Radiology", Cost = 150, TurnaroundTimeHours = 2 },
            new() { Code = "CT-HEAD", Name = "CT Scan - Head", Category = "Radiology", Cost = 800, TurnaroundTimeHours = 4 },
            new() { Code = "MRI-BRAIN", Name = "MRI - Brain", Category = "Radiology", Cost = 1500, TurnaroundTimeHours = 24 },
            new() { Code = "ECG", Name = "Electrocardiogram", Category = "Cardiology", Cost = 75, TurnaroundTimeHours = 1 },
            new() { Code = "ECHO", Name = "Echocardiogram", Category = "Cardiology", Cost = 400, TurnaroundTimeHours = 2 },
            new() { Code = "US-ABD", Name = "Ultrasound - Abdomen", Category = "Radiology", Cost = 250, TurnaroundTimeHours = 2 },
            new() { Code = "COVID", Name = "COVID-19 PCR Test", Category = "Microbiology", Cost = 100, TurnaroundTimeHours = 24 }
        };
    }
    
    private readonly IPatientService _patientService;
    private readonly IPhysicianService _physicianService;
    
    public DiagnosticTestService(IPatientService patientService, IPhysicianService physicianService)
    {
        _patientService = patientService;
        _physicianService = physicianService;
    }
    
    public async Task<DiagnosticTest> OrderTestAsync(OrderTestRequest request)
    {
        var patient = await _patientService.GetByIdAsync(request.PatientId);
        var physician = await _physicianService.GetByIdAsync(request.OrderedById);
        var testType = _testTypes.FirstOrDefault(t => t.Id == request.TestTypeId);
        
        var test = new DiagnosticTest
        {
            TestNumber = $"TST-{DateTime.UtcNow:yyyyMMdd}-{_testCounter++:D4}",
            PatientId = request.PatientId,
            PatientName = patient?.FullName ?? "Unknown",
            MRN = patient?.MRN ?? "",
            AdmissionId = request.AdmissionId,
            TestTypeId = request.TestTypeId,
            TestName = testType?.Name ?? "",
            TestCategory = testType?.Category ?? "",
            OrderedById = request.OrderedById,
            OrderedByName = physician?.FullName ?? "",
            OrderedAt = DateTime.UtcNow,
            ScheduledAt = request.ScheduledAt,
            Status = TestStatus.Ordered,
            Notes = request.Notes,
            Cost = testType?.Cost ?? 0
        };
        
        _tests.Add(test);
        return test;
    }
    
    public async Task<DiagnosticTest> RecordResultAsync(Guid testId, TestResultRequest result)
    {
        var test = await GetTestAsync(testId);
        if (test == null) throw new InvalidOperationException("Test not found");
        
        var performer = await _physicianService.GetByIdAsync(result.PerformedById);
        
        test.Results = result.Results;
        test.Interpretation = result.Interpretation;
        test.ResultFlag = result.ResultFlag;
        test.PerformedAt = DateTime.UtcNow;
        test.ResultsAt = DateTime.UtcNow;
        test.PerformedById = result.PerformedById;
        test.PerformedByName = performer?.FullName;
        test.Status = TestStatus.Completed;
        test.Notes = result.Notes;
        
        return test;
    }
    
    public Task<DiagnosticTest?> GetTestAsync(Guid id) => Task.FromResult(_tests.FirstOrDefault(t => t.Id == id));
    
    public Task<List<DiagnosticTest>> GetPatientTestsAsync(Guid patientId) =>
        Task.FromResult(_tests.Where(t => t.PatientId == patientId).OrderByDescending(t => t.OrderedAt).ToList());
    
    public Task<List<DiagnosticTest>> GetPendingTestsAsync() =>
        Task.FromResult(_tests.Where(t => t.Status == TestStatus.Ordered || t.Status == TestStatus.Scheduled || t.Status == TestStatus.InProgress).ToList());
    
    public Task<List<DiagnosticTest>> GetTestsByDateAsync(DateTime date) =>
        Task.FromResult(_tests.Where(t => t.OrderedAt.Date == date.Date).ToList());
    
    public Task<List<TestType>> GetTestTypesAsync() => Task.FromResult(_testTypes.Where(t => t.IsActive).ToList());
    
    public Task<TestType> CreateTestTypeAsync(TestType testType)
    {
        testType.Id = Guid.NewGuid();
        _testTypes.Add(testType);
        return Task.FromResult(testType);
    }
}

public class HospitalReportService : IHospitalReportService
{
    private readonly IHospitalRoomService _roomService;
    private readonly IPatientService _patientService;
    private readonly IPhysicianService _physicianService;
    private readonly IAdmissionService _admissionService;
    
    public HospitalReportService(IHospitalRoomService roomService, IPatientService patientService, IPhysicianService physicianService, IAdmissionService admissionService)
    {
        _roomService = roomService;
        _patientService = patientService;
        _physicianService = physicianService;
        _admissionService = admissionService;
    }
    
    public async Task<HospitalDashboard> GetDashboardAsync()
    {
        var rooms = await _roomService.GetAllAsync();
        var patients = await _patientService.GetAllAsync();
        var physicians = await _physicianService.GetAllAsync();
        var activeAdmissions = await _admissionService.GetActiveAdmissionsAsync();
        
        return new HospitalDashboard
        {
            TotalRooms = rooms.Count,
            AvailableRooms = rooms.Count(r => r.Status == HospitalRoomStatus.Available),
            OccupiedRooms = rooms.Count(r => r.Status == HospitalRoomStatus.Occupied),
            TotalPatients = patients.Count,
            AdmittedPatients = patients.Count(p => p.Status == PatientStatus.Admitted),
            TodayAdmissions = 8,
            TodayDischarges = 5,
            PendingTests = 12,
            ActivePhysicians = physicians.Count(p => p.Status == PhysicianStatus.Active),
            OccupancyRate = rooms.Count > 0 ? (rooms.Count(r => r.Status == HospitalRoomStatus.Occupied) * 100m / rooms.Count) : 0,
            RecentAdmissions = activeAdmissions.Take(5).ToList(),
            RoomsByType = rooms.GroupBy(r => r.Type).ToDictionary(g => g.Key, g => g.Count()),
            PatientsByDepartment = new Dictionary<string, int>
            {
                { "Internal Medicine", 25 },
                { "Surgery", 18 },
                { "Pediatrics", 12 },
                { "Emergency", 8 },
                { "OB/GYN", 6 }
            }
        };
    }
    
    public Task<AdmissionReport> GetAdmissionReportAsync(DateTime from, DateTime to)
    {
        return Task.FromResult(new AdmissionReport
        {
            FromDate = from,
            ToDate = to,
            TotalAdmissions = 156,
            EmergencyAdmissions = 42,
            ElectiveAdmissions = 114,
            AverageLengthOfStay = 4.5m,
            ByDepartment = new Dictionary<string, int> { { "Medicine", 45 }, { "Surgery", 38 }, { "Pediatrics", 28 }, { "OB/GYN", 25 }, { "Other", 20 } }
        });
    }
    
    public Task<DischargeReport> GetDischargeReportAsync(DateTime from, DateTime to)
    {
        return Task.FromResult(new DischargeReport
        {
            FromDate = from,
            ToDate = to,
            TotalDischarges = 148,
            RegularDischarges = 142,
            TransferredPatients = 4,
            DeathCount = 1,
            AMACount = 1,
            AverageLengthOfStay = 4.2m
        });
    }
    
    public Task<DiagnosticReport> GetDiagnosticReportAsync(DateTime from, DateTime to)
    {
        return Task.FromResult(new DiagnosticReport
        {
            FromDate = from,
            ToDate = to,
            TotalTests = 520,
            CompletedTests = 485,
            PendingTests = 35,
            AbnormalResults = 78,
            CriticalResults = 12,
            ByCategory = new Dictionary<string, int> { { "Hematology", 180 }, { "Chemistry", 150 }, { "Radiology", 95 }, { "Cardiology", 55 }, { "Microbiology", 40 } },
            TotalRevenue = 45000m
        });
    }
    
    public async Task<HospitalOccupancyReport> GetOccupancyReportAsync(DateTime date)
    {
        var rooms = await _roomService.GetAllAsync();
        var occupied = rooms.Count(r => r.Status == HospitalRoomStatus.Occupied);
        var totalBeds = rooms.Sum(r => r.BedCount);
        var occupiedBeds = rooms.Sum(r => r.OccupiedBeds);
        
        return new HospitalOccupancyReport
        {
            Date = date,
            TotalBeds = totalBeds,
            OccupiedBeds = occupiedBeds,
            AvailableBeds = totalBeds - occupiedBeds,
            OccupancyRate = totalBeds > 0 ? (occupiedBeds * 100m / totalBeds) : 0,
            ByRoomType = rooms.GroupBy(r => r.Type).ToDictionary(
                g => g.Key,
                g => new OccupancyByType { TotalBeds = g.Sum(r => r.BedCount), OccupiedBeds = g.Sum(r => r.OccupiedBeds) })
        };
    }
    
    public async Task<PhysicianReport> GetPhysicianReportAsync(DateTime from, DateTime to)
    {
        var physicians = await _physicianService.GetAllAsync();
        return new PhysicianReport
        {
            FromDate = from,
            ToDate = to,
            TotalPhysicians = physicians.Count,
            ActivePhysicians = physicians.Count(p => p.Status == PhysicianStatus.Active),
            ByDepartment = physicians.GroupBy(p => p.Department).ToDictionary(g => g.Key, g => g.Count()),
            BySpecialization = physicians.GroupBy(p => p.Specialization).ToDictionary(g => g.Key, g => g.Count())
        };
    }
    
    public async Task<PatientStatisticsReport> GetPatientStatisticsAsync(DateTime from, DateTime to)
    {
        var patients = await _patientService.GetAllAsync();
        return new PatientStatisticsReport
        {
            FromDate = from,
            ToDate = to,
            TotalPatients = patients.Count,
            NewPatients = 45,
            ReturningPatients = 112,
            ByGender = patients.GroupBy(p => p.Gender).ToDictionary(g => g.Key, g => g.Count()),
            ByBloodType = patients.GroupBy(p => p.BloodType).ToDictionary(g => g.Key, g => g.Count())
        };
    }
}
