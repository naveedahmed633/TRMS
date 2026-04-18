# TRMS Application Architecture — Technology Stack

**Document:** Application Architecture  
**Subject:** Technology stack and application layers  
**Version:** 1.1  
**Date:** February 2026  

---

## 1. Overview

TRMS (Time & Resource Management System) is built as a multi-tier .NET application with a web front-end, business logic layer, data access layer, and supporting jobs and integrations. This document describes the technology stack used across these layers.

---

## 2. Technology Stack Summary

| Layer | Technologies |
|-------|--------------|
| **Presentation** | ASP.NET MVC 5.3, Razor 3.3, jQuery, Bootstrap (plugins), DataTables, Font Awesome, Chart.js / Morris / NVD3 |
| **Business Logic** | .NET Framework 4.8, C#, BLL (Business Logic Layer) |
| **Data Access** | Entity Framework 6.4.4, ADO.NET, System.Data.SqlClient |
| **Database** | Microsoft SQL Server |
| **Authentication / Authorization** | ASP.NET Forms Authentication, custom Membership & Role providers |
| **Reporting / Export** | EPPlus, IronPdf, iTextSharp, Rotativa, RDLC |
| **Integration** | gRPC, Web API 5.3, OData, CSV import/export, SSIS (Alpeta), UNIS integration |
| **Cryptography** | AES (CryptograpyED), BouncyCastle, System.Security.Cryptography |

---

## 3. Presentation Layer

- **Framework:** ASP.NET MVC 5.3 (MvcApplication1)
- **View Engine:** Razor 3.3
- **UI Libraries:** jQuery 1.8.2, jQuery UI, jQuery Validation, DataTables, Bootstrap-based plugins (CSCAL), Font Awesome, Chart.js, Morris, NVD3, FullCalendar
- **Target Framework:** .NET 4.8
- **Areas (role-based modules):**
  - **HR** — Human Resources (employees, attendance, leaves, payroll, roster, shifts, reports)
  - **LM** — Line Manager (team attendance, leaves, reports)
  - **SLM** — Super Line Manager
  - **EMP** — Employee (self-service, attendance, leaves, reports)
  - **Report** — Reporting
  - **Sudo** — Sudo/admin operations
  - **SuperAdmin** — Super administration (terminals, calendars, audit log, configuration)

---

## 4. Business Logic Layer (BLL)

- **Project:** BLL (.NET 4.8)
- **Responsibilities:** Business rules, workflow, validation, reporting logic, custom authentication/authorization
- **Key Components:**
  - **TimeTuneMembershipProvider** — User validation, password management (including Super Admin / Sudo)
  - **TimeTuneRoleProvider** — Role resolution from database (AccessGroup)
  - **TimeTune** — Core business logic, AuditTrail, data operations
  - **ViewModels** — Data transfer objects for HR, LM, SLM, Admin, Reports, etc.
  - **Services** — Supporting services (e.g. email)
- **Dependencies:** Entity Framework 6.4.4, EPPlus, iTextSharp, DotNetOpenAuth, BouncyCastle

---

## 5. Data Access Layer (DLL)

- **Project:** ConsoleApplication1\DLL (DLL.csproj)
- **ORM:** Entity Framework 6.4.4
- **Database:** SQL Server (connection name: TimeTune; catalog: TRMS)
- **Context:** `DLL.Models.Context` — DbContext with entities for employees, attendance, leaves, payroll, audit, terminals (HaTransit), etc.
- **Conventions:** Table names mapped to DbSets (e.g. `audit_trail`, `ha_transit`, `employee`, `consolidated_attendance`)

---

## 6. Database

- **RDBMS:** Microsoft SQL Server
- **Primary Database:** TRMS (TimeTune connection string)
- **Secondary / Integration:** UNIS_DUHS, UNIS_DMC (UNISContext / UNISContextR2) for UNIS-related data
- **Access:** SQL authentication (configurable via connection strings in Web.config / App.config)

---

## 7. Supporting Applications & Jobs

- **Attendance Pipeline:** Windows Service (AttendancePipeLine) — processes attendance (e.g. HaTransit → persistent/consolidated logs)
- **Mark Attendance:** Console apps (MarkAttendance1, MarkContractualStaffAttendance) — mark attendance from external sources
- **Fetch / Sync Jobs:** EBI-FetchJob, UNISTT-FetchJob, UNIS-UNIS-FetchJob, UNIS-UNIS-SyncJob, UNISTT-UNISTT-FetchJob — data sync with external systems
- **CSV Jobs:** CSV-CourseAttendance-GetJob, CSV-CourseSchedule-PostJob, CSV-CourseEnrollment-PostJob, CSV-CourseTest-PostJob, CSV-ClassSchedule-PostJob — bulk import/export
- **Utilities:** AddEmployeePassword, GenerateValidationKey, CryptograpyED, SendAutomaticEmail, ServiceOnOff, Sync_BAT_File_Text_Change_Job, DeleteFakeImages, Leaves_Sandwich_Rule_Implication, Probation_Permanent_Leaves, CoursesAttendance
- **Integration:** Alpeta_TRMS_UserIntegration (SSIS), FAHE ESB Attendance Integration (see FAHE ESB Attendance Integration -ICD v2.0.pdf)

---

## 8. Integration & APIs

- **Web API:** ASP.NET Web API 5.3, Web API OData — for programmatic access
- **gRPC:** Grpc.Core, Grpc.Net.Client — for high-performance integration
- **External Systems:** UNIS (student/course data), Alpeta (biometric/devices), ESB (FAHE) — as per project configuration

---

## 9. Reporting & Documents

- **Excel:** EPPlus 7.x
- **PDF:** IronPdf, iTextSharp, Rotativa (HTML-to-PDF)
- **Reports:** RDLC (Report1.rdlc, ReportViewPage.aspx), custom PDF reports in BLL (e.g. MonthlyTimeSheet)

---

## 10. Diagram (Logical Layers)

```
┌─────────────────────────────────────────────────────────────────┐
│  Presentation (MvcApplication1)                                  │
│  ASP.NET MVC 5 | Razor | jQuery | Bootstrap | DataTables        │
│  Areas: HR | LM | SLM | EMP | Report | Sudo | SuperAdmin         │
└────────────────────────────┬────────────────────────────────────┘
                             │
┌────────────────────────────▼────────────────────────────────────┐
│  Business Logic (BLL)                                            │
│  TimeTune | Membership/Role Providers | ViewModels | Services    │
└────────────────────────────┬────────────────────────────────────┘
                             │
┌────────────────────────────▼────────────────────────────────────┐
│  Data Access (DLL)                                               │
│  Entity Framework 6 | Context | Models                           │
└────────────────────────────┬────────────────────────────────────┘
                             │
┌────────────────────────────▼────────────────────────────────────┐
│  Database                                                        │
│  SQL Server (TRMS, UNIS_*)                                       │
└─────────────────────────────────────────────────────────────────┘

  Supporting: Windows Services | Console Jobs | CSV/SSIS | gRPC/Web API
```

---

## 11. .NET Upgrade Plan (.NET Core 8+)

### 11.1 Plan to Move to Newer .NET

- **Current stack:** TRMS is built on **.NET Framework 4.8** (ASP.NET MVC 5, BLL, DLL). There is a **plan to evaluate and, where feasible, migrate to a modern LTS runtime** such as **.NET 8** (or .NET 9+) to align with Microsoft’s support lifecycle and MOC/organizational standards.
- **Scope:** The upgrade would apply to the main web application (MvcApplication1), BLL, DLL, and supporting console/worker projects. Third-party and legacy integrations (e.g. some SSIS, device integrations) may require separate compatibility assessment.
- **Benefits:** Long-term support (LTS), performance improvements, cross-platform options, and better alignment with cloud and container deployments.

### 11.2 How This Will Be Applied to MOC

- **MOC alignment:** The migration will be executed in line with MOC change and release management. MOC will be engaged for:
  - **Approval** of the target .NET version (e.g. .NET 8 LTS) and timeline.
  - **Environment impact:** Dev, UAT, and Production will be upgraded according to MOC-approved rollout (Infrastructure and UAT specs will be updated to include .NET 8 runtime and any new server requirements).
  - **Security and compliance:** Authentication (e.g. migration from Forms Auth to ASP.NET Core Identity or MOC identity provider integration), configuration, and secrets management will follow MOC security architecture (see Security Architecture document).
  - **Testing and sign-off:** UAT and security testing will be performed in MOC-aligned environments before production deployment.
- **Phasing:** A phased approach (e.g. new services on .NET 8 first, then main app) can be considered to reduce risk; exact phasing to be agreed with MOC.

---

*This document reflects the technology stack as implemented in the TRMS solution (MvcApplication1.sln) and related projects. Section 11 describes the planned .NET upgrade and MOC engagement.*
