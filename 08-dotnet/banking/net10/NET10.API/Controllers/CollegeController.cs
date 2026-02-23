using Microsoft.AspNetCore.Mvc;
using NET10.Core.Interfaces;

namespace NET10.API.Controllers;

/// <summary>
/// College Controller - Ierahkwa Sovereign Education System
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CollegeController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly ITeacherService _teacherService;
    private readonly IAttendanceService _attendanceService;
    private readonly IFeeService _feeService;
    private readonly ICollegeReportService _reportService;
    
    public CollegeController(
        IStudentService studentService,
        ITeacherService teacherService,
        IAttendanceService attendanceService,
        IFeeService feeService,
        ICollegeReportService reportService)
    {
        _studentService = studentService;
        _teacherService = teacherService;
        _attendanceService = attendanceService;
        _feeService = feeService;
        _reportService = reportService;
    }
    
    // ═══════════════════════════════════════════════════════════════
    // COLLEGE OVERVIEW
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get college overview
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<CollegeOverview>> GetOverview()
    {
        var students = await _studentService.GetAllAsync();
        var teachers = await _teacherService.GetAllAsync();
        
        return Ok(new CollegeOverview
        {
            InstitutionName = "Ierahkwa Sovereign College",
            TotalStudents = students.Count,
            ActiveStudents = students.Count(s => s.Status == StudentStatus.Active),
            TotalTeachers = teachers.Count,
            ActiveTeachers = teachers.Count(t => t.IsActive),
            CurrentSemester = "Fall 2024",
            Programs = students.Select(s => s.Program).Distinct().Count(),
            Departments = teachers.Select(t => t.Department).Distinct().Count()
        });
    }
    
    // ═══════════════════════════════════════════════════════════════
    // STUDENT MANAGEMENT
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get all students
    /// </summary>
    [HttpGet("students")]
    public async Task<ActionResult<List<Student>>> GetStudents()
    {
        var students = await _studentService.GetAllAsync();
        return Ok(students);
    }
    
    /// <summary>
    /// Get student by ID
    /// </summary>
    [HttpGet("students/{id}")]
    public async Task<ActionResult<Student>> GetStudent(Guid id)
    {
        var student = await _studentService.GetByIdAsync(id);
        if (student == null) return NotFound();
        return Ok(student);
    }
    
    /// <summary>
    /// Get student by student number
    /// </summary>
    [HttpGet("students/number/{studentNumber}")]
    public async Task<ActionResult<Student>> GetStudentByNumber(string studentNumber)
    {
        var student = await _studentService.GetByStudentNumberAsync(studentNumber);
        if (student == null) return NotFound();
        return Ok(student);
    }
    
    /// <summary>
    /// Get students by program
    /// </summary>
    [HttpGet("students/program/{program}")]
    public async Task<ActionResult<List<Student>>> GetStudentsByProgram(string program)
    {
        var students = await _studentService.GetByProgramAsync(program);
        return Ok(students);
    }
    
    /// <summary>
    /// Register new student
    /// </summary>
    [HttpPost("students")]
    public async Task<ActionResult<Student>> RegisterStudent([FromBody] Student student)
    {
        var created = await _studentService.CreateAsync(student);
        return CreatedAtAction(nameof(GetStudent), new { id = created.Id }, created);
    }
    
    /// <summary>
    /// Update student
    /// </summary>
    [HttpPut("students/{id}")]
    public async Task<ActionResult<Student>> UpdateStudent(Guid id, [FromBody] Student student)
    {
        student.Id = id;
        var updated = await _studentService.UpdateAsync(student);
        return Ok(updated);
    }
    
    /// <summary>
    /// Enroll student in courses
    /// </summary>
    [HttpPost("students/{id}/enroll")]
    public async Task<ActionResult<StudentEnrollment>> EnrollStudent(Guid id, [FromBody] EnrollmentRequest request)
    {
        var enrollment = await _studentService.EnrollAsync(id, request);
        return Ok(enrollment);
    }
    
    /// <summary>
    /// Get student grades
    /// </summary>
    [HttpGet("students/{id}/grades")]
    public async Task<ActionResult<List<StudentGrade>>> GetStudentGrades(Guid id)
    {
        var grades = await _studentService.GetGradesAsync(id);
        return Ok(grades);
    }
    
    /// <summary>
    /// Get student transcript
    /// </summary>
    [HttpGet("students/{id}/transcript")]
    public async Task<ActionResult<StudentTranscript>> GetTranscript(Guid id)
    {
        var transcript = await _studentService.GetTranscriptAsync(id);
        return Ok(transcript);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // TEACHER/FACULTY MANAGEMENT
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get all teachers
    /// </summary>
    [HttpGet("teachers")]
    public async Task<ActionResult<List<Teacher>>> GetTeachers()
    {
        var teachers = await _teacherService.GetAllAsync();
        return Ok(teachers);
    }
    
    /// <summary>
    /// Get teacher by ID
    /// </summary>
    [HttpGet("teachers/{id}")]
    public async Task<ActionResult<Teacher>> GetTeacher(Guid id)
    {
        var teacher = await _teacherService.GetByIdAsync(id);
        if (teacher == null) return NotFound();
        return Ok(teacher);
    }
    
    /// <summary>
    /// Get teachers by department
    /// </summary>
    [HttpGet("teachers/department/{department}")]
    public async Task<ActionResult<List<Teacher>>> GetTeachersByDepartment(string department)
    {
        var teachers = await _teacherService.GetByDepartmentAsync(department);
        return Ok(teachers);
    }
    
    /// <summary>
    /// Add new teacher
    /// </summary>
    [HttpPost("teachers")]
    public async Task<ActionResult<Teacher>> AddTeacher([FromBody] Teacher teacher)
    {
        var created = await _teacherService.CreateAsync(teacher);
        return CreatedAtAction(nameof(GetTeacher), new { id = created.Id }, created);
    }
    
    /// <summary>
    /// Update teacher
    /// </summary>
    [HttpPut("teachers/{id}")]
    public async Task<ActionResult<Teacher>> UpdateTeacher(Guid id, [FromBody] Teacher teacher)
    {
        teacher.Id = id;
        var updated = await _teacherService.UpdateAsync(teacher);
        return Ok(updated);
    }
    
    /// <summary>
    /// Get teacher course assignments
    /// </summary>
    [HttpGet("teachers/{id}/assignments")]
    public async Task<ActionResult<List<CourseAssignment>>> GetTeacherAssignments(Guid id)
    {
        var assignments = await _teacherService.GetAssignmentsAsync(id);
        return Ok(assignments);
    }
    
    /// <summary>
    /// Assign course to teacher
    /// </summary>
    [HttpPost("teachers/{teacherId}/assign/{courseId}")]
    public async Task<ActionResult<CourseAssignment>> AssignCourse(Guid teacherId, Guid courseId, [FromQuery] string semester)
    {
        var assignment = await _teacherService.AssignCourseAsync(teacherId, courseId, semester);
        return Ok(assignment);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // ATTENDANCE
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get student attendance
    /// </summary>
    [HttpGet("attendance/student/{studentId}")]
    public async Task<ActionResult<List<AttendanceRecord>>> GetStudentAttendance(
        Guid studentId, 
        [FromQuery] DateTime? from = null, 
        [FromQuery] DateTime? to = null)
    {
        var records = await _attendanceService.GetByStudentAsync(studentId, from, to);
        return Ok(records);
    }
    
    /// <summary>
    /// Get course attendance for a date
    /// </summary>
    [HttpGet("attendance/course/{courseId}")]
    public async Task<ActionResult<List<AttendanceRecord>>> GetCourseAttendance(Guid courseId, [FromQuery] DateTime date)
    {
        var records = await _attendanceService.GetByCourseAsync(courseId, date);
        return Ok(records);
    }
    
    /// <summary>
    /// Record attendance
    /// </summary>
    [HttpPost("attendance")]
    public async Task<ActionResult<AttendanceRecord>> RecordAttendance([FromBody] AttendanceRecord record)
    {
        var saved = await _attendanceService.RecordAsync(record);
        return Ok(saved);
    }
    
    /// <summary>
    /// Bulk record attendance
    /// </summary>
    [HttpPost("attendance/bulk")]
    public async Task<ActionResult<List<AttendanceRecord>>> BulkRecordAttendance([FromBody] List<AttendanceRecord> records)
    {
        var saved = await _attendanceService.BulkRecordAsync(records);
        return Ok(saved);
    }
    
    /// <summary>
    /// Get attendance summary
    /// </summary>
    [HttpGet("attendance/summary/{studentId}")]
    public async Task<ActionResult<AttendanceSummary>> GetAttendanceSummary(Guid studentId, [FromQuery] string semester)
    {
        var summary = await _attendanceService.GetSummaryAsync(studentId, semester);
        return Ok(summary);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // FEE MANAGEMENT
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get fee structures
    /// </summary>
    [HttpGet("fees/structures")]
    public async Task<ActionResult<List<FeeStructure>>> GetFeeStructures()
    {
        var structures = await _feeService.GetFeeStructuresAsync();
        return Ok(structures);
    }
    
    /// <summary>
    /// Get student fees
    /// </summary>
    [HttpGet("fees/student/{studentId}")]
    public async Task<ActionResult<List<StudentFee>>> GetStudentFees(Guid studentId)
    {
        var fees = await _feeService.GetStudentFeesAsync(studentId);
        return Ok(fees);
    }
    
    /// <summary>
    /// Generate fee for student
    /// </summary>
    [HttpPost("fees/generate/{studentId}")]
    public async Task<ActionResult<StudentFee>> GenerateFee(Guid studentId, [FromQuery] string semester)
    {
        var fee = await _feeService.GenerateFeeAsync(studentId, semester);
        return Ok(fee);
    }
    
    /// <summary>
    /// Record fee payment
    /// </summary>
    [HttpPost("fees/{feeId}/payment")]
    public async Task<ActionResult<FeePayment>> RecordPayment(Guid feeId, [FromBody] FeePayment payment)
    {
        var recorded = await _feeService.RecordPaymentAsync(feeId, payment);
        return Ok(recorded);
    }
    
    /// <summary>
    /// Get student balance
    /// </summary>
    [HttpGet("fees/balance/{studentId}")]
    public async Task<ActionResult<decimal>> GetStudentBalance(Guid studentId)
    {
        var balance = await _feeService.GetBalanceAsync(studentId);
        return Ok(new { balance });
    }
    
    /// <summary>
    /// Get overdue fees
    /// </summary>
    [HttpGet("fees/overdue")]
    public async Task<ActionResult<List<StudentFee>>> GetOverdueFees()
    {
        var fees = await _feeService.GetOverdueFeesAsync();
        return Ok(fees);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // REPORTS
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get enrollment report
    /// </summary>
    [HttpGet("reports/enrollment")]
    public async Task<ActionResult<EnrollmentReport>> GetEnrollmentReport([FromQuery] string semester = "Fall 2024")
    {
        var report = await _reportService.GetEnrollmentReportAsync(semester);
        return Ok(report);
    }
    
    /// <summary>
    /// Get attendance report
    /// </summary>
    [HttpGet("reports/attendance")]
    public async Task<ActionResult<AttendanceReport>> GetAttendanceReport(
        [FromQuery] string semester = "Fall 2024", 
        [FromQuery] string? department = null)
    {
        var report = await _reportService.GetAttendanceReportAsync(semester, department);
        return Ok(report);
    }
    
    /// <summary>
    /// Get academic performance report
    /// </summary>
    [HttpGet("reports/performance")]
    public async Task<ActionResult<AcademicPerformanceReport>> GetPerformanceReport([FromQuery] string semester = "Fall 2024")
    {
        var report = await _reportService.GetPerformanceReportAsync(semester);
        return Ok(report);
    }
    
    /// <summary>
    /// Get fee collection report
    /// </summary>
    [HttpGet("reports/fees")]
    public async Task<ActionResult<FeeCollectionReport>> GetFeeReport([FromQuery] string semester = "Fall 2024")
    {
        var report = await _reportService.GetFeeReportAsync(semester);
        return Ok(report);
    }
    
    /// <summary>
    /// Get department report
    /// </summary>
    [HttpGet("reports/department/{department}")]
    public async Task<ActionResult<DepartmentReport>> GetDepartmentReport(string department)
    {
        var report = await _reportService.GetDepartmentReportAsync(department);
        return Ok(report);
    }
}

// ═══════════════════════════════════════════════════════════════
// COLLEGE API MODELS
// ═══════════════════════════════════════════════════════════════

public class CollegeOverview
{
    public string InstitutionName { get; set; } = string.Empty;
    public int TotalStudents { get; set; }
    public int ActiveStudents { get; set; }
    public int TotalTeachers { get; set; }
    public int ActiveTeachers { get; set; }
    public string CurrentSemester { get; set; } = string.Empty;
    public int Programs { get; set; }
    public int Departments { get; set; }
}
