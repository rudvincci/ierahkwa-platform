using Microsoft.AspNetCore.Mvc;
using NET10.Core.Interfaces;

namespace NET10.API.Controllers;

/// <summary>
/// Hospital Controller - Ierahkwa Sovereign Health System
/// Patient Records, Admissions, Diagnostics & Reporting
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class HospitalController : ControllerBase
{
    private readonly IHospitalRoomService _roomService;
    private readonly IPhysicianService _physicianService;
    private readonly IDiagnosisService _diagnosisService;
    private readonly IPatientService _patientService;
    private readonly IAdmissionService _admissionService;
    private readonly IDiagnosticTestService _testService;
    private readonly IHospitalReportService _reportService;
    
    public HospitalController(
        IHospitalRoomService roomService,
        IPhysicianService physicianService,
        IDiagnosisService diagnosisService,
        IPatientService patientService,
        IAdmissionService admissionService,
        IDiagnosticTestService testService,
        IHospitalReportService reportService)
    {
        _roomService = roomService;
        _physicianService = physicianService;
        _diagnosisService = diagnosisService;
        _patientService = patientService;
        _admissionService = admissionService;
        _testService = testService;
        _reportService = reportService;
    }
    
    // ═══════════════════════════════════════════════════════════════
    // DASHBOARD
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get hospital dashboard
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<HospitalDashboard>> GetDashboard()
    {
        var dashboard = await _reportService.GetDashboardAsync();
        return Ok(dashboard);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // ROOMS
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get all rooms
    /// </summary>
    [HttpGet("rooms")]
    public async Task<ActionResult<List<HospitalRoom>>> GetRooms()
    {
        var rooms = await _roomService.GetAllAsync();
        return Ok(rooms);
    }
    
    /// <summary>
    /// Get room by ID
    /// </summary>
    [HttpGet("rooms/{id}")]
    public async Task<ActionResult<HospitalRoom>> GetRoom(Guid id)
    {
        var room = await _roomService.GetByIdAsync(id);
        if (room == null) return NotFound();
        return Ok(room);
    }
    
    /// <summary>
    /// Get available rooms
    /// </summary>
    [HttpGet("rooms/available")]
    public async Task<ActionResult<List<HospitalRoom>>> GetAvailableRooms()
    {
        var rooms = await _roomService.GetAvailableAsync();
        return Ok(rooms);
    }
    
    /// <summary>
    /// Get rooms by type
    /// </summary>
    [HttpGet("rooms/type/{type}")]
    public async Task<ActionResult<List<HospitalRoom>>> GetRoomsByType(HospitalRoomType type)
    {
        var rooms = await _roomService.GetByTypeAsync(type);
        return Ok(rooms);
    }
    
    /// <summary>
    /// Create room
    /// </summary>
    [HttpPost("rooms")]
    public async Task<ActionResult<HospitalRoom>> CreateRoom([FromBody] HospitalRoom room)
    {
        var created = await _roomService.CreateAsync(room);
        return CreatedAtAction(nameof(GetRoom), new { id = created.Id }, created);
    }
    
    /// <summary>
    /// Update room
    /// </summary>
    [HttpPut("rooms/{id}")]
    public async Task<ActionResult<HospitalRoom>> UpdateRoom(Guid id, [FromBody] HospitalRoom room)
    {
        room.Id = id;
        var updated = await _roomService.UpdateAsync(room);
        return Ok(updated);
    }
    
    /// <summary>
    /// Set room status
    /// </summary>
    [HttpPost("rooms/{id}/status")]
    public async Task<ActionResult> SetRoomStatus(Guid id, [FromQuery] HospitalRoomStatus status)
    {
        var result = await _roomService.SetStatusAsync(id, status);
        if (!result) return NotFound();
        return Ok(new { id, status = status.ToString() });
    }
    
    /// <summary>
    /// Get room occupancy report
    /// </summary>
    [HttpGet("rooms/occupancy")]
    public async Task<ActionResult<RoomOccupancyReport>> GetRoomOccupancy()
    {
        var report = await _roomService.GetOccupancyReportAsync();
        return Ok(report);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // PHYSICIANS
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get all physicians
    /// </summary>
    [HttpGet("physicians")]
    public async Task<ActionResult<List<Physician>>> GetPhysicians()
    {
        var physicians = await _physicianService.GetAllAsync();
        return Ok(physicians);
    }
    
    /// <summary>
    /// Get physician by ID
    /// </summary>
    [HttpGet("physicians/{id}")]
    public async Task<ActionResult<Physician>> GetPhysician(Guid id)
    {
        var physician = await _physicianService.GetByIdAsync(id);
        if (physician == null) return NotFound();
        return Ok(physician);
    }
    
    /// <summary>
    /// Get physician by license number
    /// </summary>
    [HttpGet("physicians/license/{licenseNumber}")]
    public async Task<ActionResult<Physician>> GetPhysicianByLicense(string licenseNumber)
    {
        var physician = await _physicianService.GetByLicenseNumberAsync(licenseNumber);
        if (physician == null) return NotFound();
        return Ok(physician);
    }
    
    /// <summary>
    /// Get physicians by department
    /// </summary>
    [HttpGet("physicians/department/{department}")]
    public async Task<ActionResult<List<Physician>>> GetPhysiciansByDepartment(string department)
    {
        var physicians = await _physicianService.GetByDepartmentAsync(department);
        return Ok(physicians);
    }
    
    /// <summary>
    /// Create physician
    /// </summary>
    [HttpPost("physicians")]
    public async Task<ActionResult<Physician>> CreatePhysician([FromBody] Physician physician)
    {
        var created = await _physicianService.CreateAsync(physician);
        return CreatedAtAction(nameof(GetPhysician), new { id = created.Id }, created);
    }
    
    /// <summary>
    /// Update physician
    /// </summary>
    [HttpPut("physicians/{id}")]
    public async Task<ActionResult<Physician>> UpdatePhysician(Guid id, [FromBody] Physician physician)
    {
        physician.Id = id;
        var updated = await _physicianService.UpdateAsync(physician);
        return Ok(updated);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // DIAGNOSES
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get all diagnoses
    /// </summary>
    [HttpGet("diagnoses")]
    public async Task<ActionResult<List<Diagnosis>>> GetDiagnoses()
    {
        var diagnoses = await _diagnosisService.GetAllAsync();
        return Ok(diagnoses);
    }
    
    /// <summary>
    /// Get diagnosis by ID
    /// </summary>
    [HttpGet("diagnoses/{id}")]
    public async Task<ActionResult<Diagnosis>> GetDiagnosis(Guid id)
    {
        var diagnosis = await _diagnosisService.GetByIdAsync(id);
        if (diagnosis == null) return NotFound();
        return Ok(diagnosis);
    }
    
    /// <summary>
    /// Get diagnoses by category
    /// </summary>
    [HttpGet("diagnoses/category/{category}")]
    public async Task<ActionResult<List<Diagnosis>>> GetDiagnosesByCategory(string category)
    {
        var diagnoses = await _diagnosisService.GetByCategoryAsync(category);
        return Ok(diagnoses);
    }
    
    /// <summary>
    /// Search diagnoses
    /// </summary>
    [HttpGet("diagnoses/search")]
    public async Task<ActionResult<List<Diagnosis>>> SearchDiagnoses([FromQuery] string term)
    {
        var diagnoses = await _diagnosisService.SearchAsync(term);
        return Ok(diagnoses);
    }
    
    /// <summary>
    /// Create diagnosis
    /// </summary>
    [HttpPost("diagnoses")]
    public async Task<ActionResult<Diagnosis>> CreateDiagnosis([FromBody] Diagnosis diagnosis)
    {
        var created = await _diagnosisService.CreateAsync(diagnosis);
        return CreatedAtAction(nameof(GetDiagnosis), new { id = created.Id }, created);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // PATIENTS
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get all patients
    /// </summary>
    [HttpGet("patients")]
    public async Task<ActionResult<List<Patient>>> GetPatients()
    {
        var patients = await _patientService.GetAllAsync();
        return Ok(patients);
    }
    
    /// <summary>
    /// Get patient by ID
    /// </summary>
    [HttpGet("patients/{id}")]
    public async Task<ActionResult<Patient>> GetPatient(Guid id)
    {
        var patient = await _patientService.GetByIdAsync(id);
        if (patient == null) return NotFound();
        return Ok(patient);
    }
    
    /// <summary>
    /// Get patient by MRN
    /// </summary>
    [HttpGet("patients/mrn/{mrn}")]
    public async Task<ActionResult<Patient>> GetPatientByMRN(string mrn)
    {
        var patient = await _patientService.GetByMRNAsync(mrn);
        if (patient == null) return NotFound();
        return Ok(patient);
    }
    
    /// <summary>
    /// Get admitted patients
    /// </summary>
    [HttpGet("patients/admitted")]
    public async Task<ActionResult<List<Patient>>> GetAdmittedPatients()
    {
        var patients = await _patientService.GetAdmittedAsync();
        return Ok(patients);
    }
    
    /// <summary>
    /// Register new patient
    /// </summary>
    [HttpPost("patients")]
    public async Task<ActionResult<Patient>> RegisterPatient([FromBody] Patient patient)
    {
        var created = await _patientService.RegisterAsync(patient);
        return CreatedAtAction(nameof(GetPatient), new { id = created.Id }, created);
    }
    
    /// <summary>
    /// Update patient
    /// </summary>
    [HttpPut("patients/{id}")]
    public async Task<ActionResult<Patient>> UpdatePatient(Guid id, [FromBody] Patient patient)
    {
        patient.Id = id;
        var updated = await _patientService.UpdateAsync(patient);
        return Ok(updated);
    }
    
    /// <summary>
    /// Get patient medical record
    /// </summary>
    [HttpGet("patients/{id}/record")]
    public async Task<ActionResult<PatientMedicalRecord>> GetPatientRecord(Guid id)
    {
        var record = await _patientService.GetMedicalRecordAsync(id);
        return Ok(record);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // ADMISSIONS
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get active admissions
    /// </summary>
    [HttpGet("admissions")]
    public async Task<ActionResult<List<PatientAdmission>>> GetAdmissions()
    {
        var admissions = await _admissionService.GetActiveAdmissionsAsync();
        return Ok(admissions);
    }
    
    /// <summary>
    /// Get admission by ID
    /// </summary>
    [HttpGet("admissions/{id}")]
    public async Task<ActionResult<PatientAdmission>> GetAdmission(Guid id)
    {
        var admission = await _admissionService.GetAdmissionAsync(id);
        if (admission == null) return NotFound();
        return Ok(admission);
    }
    
    /// <summary>
    /// Admit patient
    /// </summary>
    [HttpPost("admissions/admit")]
    public async Task<ActionResult<PatientAdmission>> AdmitPatient([FromBody] AdmitPatientRequest request)
    {
        try
        {
            var admission = await _admissionService.AdmitPatientAsync(request);
            return Ok(admission);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Discharge patient
    /// </summary>
    [HttpPost("admissions/{id}/discharge")]
    public async Task<ActionResult<PatientAdmission>> DischargePatient(Guid id, [FromBody] DischargeRequest request)
    {
        try
        {
            var admission = await _admissionService.DischargePatientAsync(id, request);
            return Ok(admission);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Transfer patient to another room
    /// </summary>
    [HttpPost("admissions/{id}/transfer")]
    public async Task<ActionResult<PatientAdmission>> TransferPatient(Guid id, [FromQuery] Guid roomId, [FromQuery] string reason)
    {
        try
        {
            var admission = await _admissionService.TransferPatientAsync(id, roomId, reason);
            return Ok(admission);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    // ═══════════════════════════════════════════════════════════════
    // DIAGNOSTIC TESTS
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get test types
    /// </summary>
    [HttpGet("tests/types")]
    public async Task<ActionResult<List<TestType>>> GetTestTypes()
    {
        var types = await _testService.GetTestTypesAsync();
        return Ok(types);
    }
    
    /// <summary>
    /// Get pending tests
    /// </summary>
    [HttpGet("tests/pending")]
    public async Task<ActionResult<List<DiagnosticTest>>> GetPendingTests()
    {
        var tests = await _testService.GetPendingTestsAsync();
        return Ok(tests);
    }
    
    /// <summary>
    /// Get test by ID
    /// </summary>
    [HttpGet("tests/{id}")]
    public async Task<ActionResult<DiagnosticTest>> GetTest(Guid id)
    {
        var test = await _testService.GetTestAsync(id);
        if (test == null) return NotFound();
        return Ok(test);
    }
    
    /// <summary>
    /// Get patient tests
    /// </summary>
    [HttpGet("tests/patient/{patientId}")]
    public async Task<ActionResult<List<DiagnosticTest>>> GetPatientTests(Guid patientId)
    {
        var tests = await _testService.GetPatientTestsAsync(patientId);
        return Ok(tests);
    }
    
    /// <summary>
    /// Order diagnostic test
    /// </summary>
    [HttpPost("tests/order")]
    public async Task<ActionResult<DiagnosticTest>> OrderTest([FromBody] OrderTestRequest request)
    {
        var test = await _testService.OrderTestAsync(request);
        return Ok(test);
    }
    
    /// <summary>
    /// Record test result
    /// </summary>
    [HttpPost("tests/{id}/result")]
    public async Task<ActionResult<DiagnosticTest>> RecordTestResult(Guid id, [FromBody] TestResultRequest result)
    {
        try
        {
            var test = await _testService.RecordResultAsync(id, result);
            return Ok(test);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    // ═══════════════════════════════════════════════════════════════
    // REPORTS
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get admission report
    /// </summary>
    [HttpGet("reports/admissions")]
    public async Task<ActionResult<AdmissionReport>> GetAdmissionReport(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var report = await _reportService.GetAdmissionReportAsync(
            from ?? DateTime.UtcNow.AddDays(-30), to ?? DateTime.UtcNow);
        return Ok(report);
    }
    
    /// <summary>
    /// Get discharge report
    /// </summary>
    [HttpGet("reports/discharges")]
    public async Task<ActionResult<DischargeReport>> GetDischargeReport(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var report = await _reportService.GetDischargeReportAsync(
            from ?? DateTime.UtcNow.AddDays(-30), to ?? DateTime.UtcNow);
        return Ok(report);
    }
    
    /// <summary>
    /// Get diagnostic report
    /// </summary>
    [HttpGet("reports/diagnostics")]
    public async Task<ActionResult<DiagnosticReport>> GetDiagnosticReport(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var report = await _reportService.GetDiagnosticReportAsync(
            from ?? DateTime.UtcNow.AddDays(-30), to ?? DateTime.UtcNow);
        return Ok(report);
    }
    
    /// <summary>
    /// Get occupancy report
    /// </summary>
    [HttpGet("reports/occupancy")]
    public async Task<ActionResult<HospitalOccupancyReport>> GetOccupancyReport([FromQuery] DateTime? date = null)
    {
        var report = await _reportService.GetOccupancyReportAsync(date ?? DateTime.UtcNow);
        return Ok(report);
    }
    
    /// <summary>
    /// Get physician report
    /// </summary>
    [HttpGet("reports/physicians")]
    public async Task<ActionResult<PhysicianReport>> GetPhysicianReport(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var report = await _reportService.GetPhysicianReportAsync(
            from ?? DateTime.UtcNow.AddDays(-30), to ?? DateTime.UtcNow);
        return Ok(report);
    }
    
    /// <summary>
    /// Get patient statistics
    /// </summary>
    [HttpGet("reports/patients")]
    public async Task<ActionResult<PatientStatisticsReport>> GetPatientStatistics(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var report = await _reportService.GetPatientStatisticsAsync(
            from ?? DateTime.UtcNow.AddDays(-30), to ?? DateTime.UtcNow);
        return Ok(report);
    }
}
