# Citizenship Status Progression Implementation

## Overview

The citizenship status progression system allows citizens to progress through three status levels:
1. **Probationary** (Initial - First 2 years)
2. **Resident** (Years 3-5)
3. **Citizen** (After year 5 - Permanent)

## Implementation Summary

### Core Components

#### 1. Models & Enums
- `CitizenshipStatus` enum (Probationary, Resident, Citizen)
- `CitizenshipStatusDetailsDto` - Current status information
- `StatusProgressionEligibilityDto` - Eligibility check results
- `StatusProgressionApplicationDto` - Progression application details

#### 2. Database Schema
- `citizenship_statuses` table - Tracks current citizenship status
- `status_progression_applications` table - Tracks progression applications
- Both tables are tenant-scoped and linked to applications

#### 3. Services

**`IDocumentValidityCalculator`**
- Calculates document expiration dates based on citizenship status
- Validity periods:
  - Probationary: 2 years (all documents)
  - Resident: 3 years (all documents)
  - Citizen: Variable (Passport: 10yrs, Driver License: 7yrs, ID Cards: 5yrs, Vehicle Tags: 1yr)

**`ICitizenshipStatusService`**
- `GetStatusByEmailAsync()` - Get status by email
- `GetStatusByApplicationIdAsync()` - Get status by application ID
- `CreateOrUpdateStatusAsync()` - Create/update status on approval
- `IsEligibleForStatusProgressionAsync()` - Check eligibility
- `GetStatusDetailsAsync()` - Get detailed status information

**`IStatusProgressionService`**
- `CheckEligibilityAsync()` - Check if eligible for progression
- `SubmitProgressionApplicationAsync()` - Submit progression application
- `GetProgressionApplicationsAsync()` - Get all progression applications
- `ApproveProgressionApplicationAsync()` - Approve and update status

#### 4. Document Issuance Integration
- `IssuePassportAsync()` - Uses status-based validity
- `IssueIdCardAsync()` - Uses status-based validity
- `IssueVehicleTagAsync()` - Uses status-based validity
- All methods automatically calculate expiration based on current citizenship status

#### 5. UI Components

**Citizen Portal:**
- `/citizen/status-progression` - Status progression application page
  - Shows current status
  - Displays eligibility for Resident/Citizen status
  - Allows submission of progression applications
  - Lists all progression applications

**Admin Endpoints:**
- `GET /gov/progression-applications` - List all progression applications for review
- `POST /gov/progression-applications/{id}/approve` - Approve a progression application

### Workflow

#### Initial Application Approval
1. Application is approved → Status set to "PassportIssued"
2. `CitizenshipStatusRow` is created with "Probationary" status
3. Status expires after 2 years
4. Documents issued with 2-year validity

#### Probationary → Resident
1. Citizen completes 2 years as Probationary
2. Citizen applies via `/citizen/status-progression`
3. Application is submitted and stored in `status_progression_applications`
4. Admin reviews and approves via `/gov/progression-applications/{id}/approve`
5. Status updated to "Resident", expires after 3 years
6. Documents re-issued with 3-year validity

#### Resident → Citizen
1. Citizen completes 5 total years (2 Probationary + 3 Resident)
2. Citizen applies via `/citizen/status-progression`
3. Application is submitted and stored
4. Admin reviews and approves
5. Status updated to "Citizen" (permanent)
6. Documents issued with variable validity (Passport: 10yrs, Driver License: 7yrs, etc.)

### Eligibility Rules

**Probationary → Resident:**
- Must be in Probationary status
- Must have completed 2 years
- Probationary status must be expired (or expiring soon)

**Resident → Citizen:**
- Must be in Resident status
- Must have completed 5 total years
- Resident status must be expired (or expiring soon)

### Database Tables

#### `citizenship_statuses`
```sql
- Id (uuid, PK)
- TenantId (varchar(128))
- Email (varchar(256), unique with TenantId)
- Status (varchar(64)) - "Probationary", "Resident", "Citizen"
- ApplicationId (uuid, FK to citizenship_applications)
- StatusGrantedAt (timestamp)
- StatusExpiresAt (timestamp, nullable) - null for Citizen
- YearsCompleted (integer)
- CreatedAt, UpdatedAt (timestamps)
```

#### `status_progression_applications`
```sql
- Id (uuid, PK)
- TenantId (varchar(128))
- CitizenshipStatusId (uuid, FK to citizenship_statuses)
- ApplicationNumber (varchar(64), unique with TenantId)
- TargetStatus (varchar(64)) - "Resident" or "Citizen"
- Status (varchar(64)) - "Submitted", "Approved", "Rejected"
- YearsCompletedAtApplication (integer)
- CreatedAt, UpdatedAt (timestamps)
```

### API Endpoints

#### Citizen Endpoints
- `GET /citizen/status-progression` - Status progression page (UI)
- (Internal) Uses `IStatusProgressionService` methods

#### Admin Endpoints
- `GET /gov/progression-applications` - List all progression applications
- `POST /gov/progression-applications/{id}/approve` - Approve progression application

### Integration Points

1. **Application Approval** (`CitizenshipBackofficeService.IssuePassportAsync`)
   - Automatically creates Probationary status when passport is issued

2. **Document Issuance** (All issuance methods)
   - Automatically uses status-based validity calculation
   - Expiration dates calculated based on current citizenship status

3. **Status Progression Approval**
   - Updates citizenship status
   - Recalculates expiration dates
   - Documents should be re-issued with new validity periods

### Next Steps (Future Work)

1. **Document Re-issuance on Status Progression**
   - Automatically re-issue documents when status progresses
   - Update expiration dates for existing documents

2. **Admin UI for Progression Applications**
   - Create admin page to review progression applications
   - Show eligibility details and application history

3. **Automatic Eligibility Notifications**
   - Notify citizens when they become eligible for progression
   - Email/SignalR notifications

4. **Status Progression Timeline**
   - Show timeline of status changes
   - Display progression history

5. **Document Renewal Workflow**
   - Allow renewal of expired documents while maintaining status
   - Separate from status progression

### Testing

To test the status progression system:

1. **Create an approved application:**
   ```bash
   POST /dev/seed/citizenship-manual
   ```

2. **Issue a passport** (creates Probationary status):
   ```bash
   POST /dev/issue/passport?applicationNumber=APP-...
   ```

3. **Check eligibility:**
   - Navigate to `/citizen/status-progression`
   - View current status and eligibility

4. **Submit progression application:**
   - Click "Apply for Resident Status" (after 2 years)
   - Application is created with "Submitted" status

5. **Approve progression application:**
   ```bash
   POST /gov/progression-applications/{id}/approve
   ```

6. **Verify status update:**
   - Check `/citizen/status-progression` to see updated status
   - Issue new documents to verify 3-year validity

### Files Created/Modified

**New Files:**
- `Mamey.Portal.Citizenship.Application/Models/CitizenshipStatus.cs`
- `Mamey.Portal.Citizenship.Application/Models/CitizenshipStatusDetailsDto.cs`
- `Mamey.Portal.Citizenship.Application/Models/StatusProgressionEligibilityDto.cs`
- `Mamey.Portal.Citizenship.Application/Models/StatusProgressionApplicationDto.cs`
- `Mamey.Portal.Citizenship.Application/Services/IDocumentValidityCalculator.cs`
- `Mamey.Portal.Citizenship.Application/Services/DocumentValidityCalculator.cs`
- `Mamey.Portal.Citizenship.Application/Services/IStatusProgressionService.cs`
- `Mamey.Portal.Citizenship.Infrastructure/Services/CitizenshipStatusService.cs`
- `Mamey.Portal.Citizenship.Infrastructure/Services/StatusProgressionService.cs`
- `Mamey.Portal.Citizenship.Infrastructure/Persistence/CitizenshipStatusRow.cs`
- `Mamey.Portal.Citizenship.Infrastructure/Persistence/StatusProgressionApplicationRow.cs`
- `Mamey.Portal.Web/Pages/Citizen/StatusProgression.razor`

**Modified Files:**
- `Mamey.Portal.Citizenship.Infrastructure/Persistence/CitizenshipDbContext.cs`
- `Mamey.Portal.Citizenship.Infrastructure/Services/CitizenshipBackofficeService.cs`
- `Mamey.Portal.Citizenship.Application/Services/ICitizenshipStatusService.cs`
- `Mamey.Portal.Web/Program.cs`
- `Mamey.Portal.Web/Pages/Citizen/Dashboard.razor`

### Status

✅ **Core infrastructure complete**
- Status tracking implemented
- Document validity calculation implemented
- Status progression service implemented
- Citizen UI implemented
- Admin endpoints implemented

🚧 **Remaining work:**
- Admin UI for reviewing progression applications
- Automatic document re-issuance on status progression
- Status progression notifications
- Document renewal workflow


