# Citizenship Application Package Analysis

## Application Package Reference

**Package Location**: `/Users/manolo/Library/CloudStorage/OneDrive-SharedLibraries-IerahkwaneKanienkeGovernment/Citizenship Services Division - Documents/AdoptionPackage/Individual Files/`

## Analysis Date

_To be filled in when analysis is performed_

## Application Package Contents

The complete application package includes the following forms and documents:

1. **CIT-001-A Citizenship Application.pdf** - Main application form (PRIMARY FOCUS)
2. **CIT-001-B Treaty Acknowledgment.pdf** - Treaty acknowledgment form
3. **CIT-001-C Affidavit of Allegiance.pdf** - Affidavit of allegiance form
4. **CIT-001-D Supporting Document Checklist.pdf** - Document checklist
5. **CIT-001-E Biometric Enrollment Authorization Form.pdf** - Biometric authorization
6. **CIT-001-F Intake Review Form.pdf** - Intake review form (for government agents)
7. **CIT-001-G Declaration of Understanding.pdf** - Declaration form
8. **CIT-001-H Consent to Verification and Data Processing.pdf** - Consent form
9. **CIT-GUIDE-001 Citizen Applicant Handbook.pdf** - Applicant handbook/guide
10. **Application Packet Cover Letter.pdf** - Cover letter
11. **CIT-001 Citizenship Policy.pdf** - Citizenship policy document
12. **INKG NDA.pdf** - Non-disclosure agreement

## Application Flow / Workflow

_Describe the order and relationship between forms:_

1. [Form/Step Name] → [Next Form/Step]
2. [Form/Step Name] → [Next Form/Step]
_..._

## Form Analysis

### Form 1: CIT-001-A Citizenship Application (Main Form)

**File**: `CIT-001-A Citizenship Application.pdf`

#### Form Structure

**Section 1: [Section Name]**
_Describe each section of the PDF form_

**Fields in this section:**
- Field Name | Type | Required | Validation Rules | Notes

**Section 2: [Section Name]**
_Continue for all sections..._

#### Field Mapping

| PDF Field Name | Current Form Field | Status | Notes |
|----------------|-------------------|--------|-------|
| | | | |

**Status Legend:**
- ✅ Matches
- ⚠️ Needs modification
- ❌ Missing
- ➕ New field needed

#### Missing Fields

_List all fields from this PDF that are not in current form_

#### Fields to Modify

_List all current fields that need changes to match this PDF_

#### Validation Rules

_Extract all validation rules from this PDF_

#### Conditional Logic

_Note any conditional fields or sections (e.g., "If X, then show Y")_

#### Layout and Organization

_Describe the layout, grouping, and order of fields in the PDF_

#### Special Requirements

_Any special formatting, instructions, or requirements from the PDF_

---

### Form 2: CIT-001-B Treaty Acknowledgment

**File**: `CIT-001-B Treaty Acknowledgment.pdf`

_[Repeat structure above for each form]_

---

### Form 3: CIT-001-C Affidavit of Allegiance

**File**: `CIT-001-C Affidavit of Allegiance.pdf`

_[Repeat structure above for each form]_

---

### Form 4: CIT-001-D Supporting Document Checklist

**File**: `CIT-001-D Supporting Document Checklist.pdf`

_[Repeat structure above for each form]_

---

### Form 5: CIT-001-E Biometric Enrollment Authorization Form

**File**: `CIT-001-E Biometric Enrollment Authorization Form.pdf`

_[Repeat structure above for each form]_

---

### Form 6: CIT-001-G Declaration of Understanding

**File**: `CIT-001-G Declaration of Understanding.pdf`

_[Repeat structure above for each form]_

---

### Form 7: CIT-001-H Consent to Verification and Data Processing

**File**: `CIT-001-H Consent to Verification and Data Processing.pdf`

_[Repeat structure above for each form]_

---

### Form 8: CIT-001-F Intake Review Form (Agent-Only)

**File**: `CIT-001-F Intake Review Form.pdf`

**Note**: This form is for government agents, not applicants.

_[Repeat structure above for each form]_

---

## Supporting Documents

### CIT-GUIDE-001 Citizen Applicant Handbook

**File**: `CIT-GUIDE-001 Citizen Applicant Handbook.pdf`

**Purpose**: Reference guide for applicants

**Integration**: Should be available as help/guidance during online application process

**Key Sections:**
- [Section 1]
- [Section 2]
_..._

### Application Packet Cover Letter

**File**: `Application Packet Cover Letter.pdf`

**Purpose**: Introduction/cover letter for application package

**Integration**: Could be displayed as introduction page before starting application

### CIT-001 Citizenship Policy

**File**: `CIT-001 Citizenship Policy.pdf`

**Purpose**: Official citizenship policy document

**Integration**: Should be linked/referenced during application process

### INKG NDA

**File**: `INKG NDA.pdf`

**Purpose**: Non-disclosure agreement

**Integration**: May need to be signed/acknowledged as part of application

---

## Cross-Form Relationships

_Note any fields that appear in multiple forms or need to be consistent across forms:_

- Field X appears in Form A and Form B → must be consistent
- Field Y in Form A determines visibility of Section Z in Form B
_..._

## Unified Data Model

_Proposed unified data model that captures all form data:_

```csharp
// Proposed structure
public class CitizenshipApplicationPackage
{
    // CIT-001-A fields
    // CIT-001-B fields
    // CIT-001-C fields
    // CIT-001-D fields
    // CIT-001-E fields
    // CIT-001-G fields
    // CIT-001-H fields
}
```

## Implementation Notes

### Multi-Step Form Design

_Notes for implementing multi-step application flow:_

- Step 1: CIT-001-A (Main Application)
- Step 2: CIT-001-B (Treaty Acknowledgment)
- Step 3: CIT-001-C (Affidavit of Allegiance)
- Step 4: CIT-001-D (Supporting Document Checklist)
- Step 5: CIT-001-E (Biometric Enrollment Authorization)
- Step 6: CIT-001-G (Declaration of Understanding)
- Step 7: CIT-001-H (Consent to Verification and Data Processing)
- Step 8: Review & Submit

### Form Generation

_Notes for generating PDF versions of completed forms:_

- Generate PDF for each completed form
- Store in MinIO
- Link to application record
- Allow download by applicant

### Agent Intake Form

_Notes for implementing CIT-001-F (Intake Review Form) for agents:_

- Separate UI/route for agent intake review
- Link to application workflow
- Agent-only access

