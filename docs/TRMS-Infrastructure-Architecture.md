# TRMS Infrastructure Architecture — HA N-Tier (MOC-Aligned)

**Document:** Infrastructure Architecture  
**Subject:** High availability and N-tier deployment model (aligned with MOC practices)  
**Version:** 1.1  
**Date:** February 2026  

---

## 1. Overview

TRMS infrastructure is designed as an **N-tier, High Availability (HA)** architecture in line with Ministry of IT/Communications (MOC) and enterprise standards. This document describes the tiers, components, HA considerations, and environment specifications (including UAT).

---

## 2. N-Tier Architecture Summary

| Tier | Role | Typical Components |
|------|------|---------------------|
| **Client / Browser** | User access | HTTPS, modern browsers |
| **Web Tier (DMZ)** | Presentation, static assets, SSL termination | IIS, ASP.NET MVC app (MvcApplication1); **deployed inside DMZ** |
| **Application Tier** | Business logic, session, auth | **Dedicated load-balanced application servers** hosting BLL (see §3.3) |
| **Data Tier** | Persistence, reporting | SQL Server (TRMS, UNIS databases) |
| **Integration / Jobs Tier** | Batch, sync, external systems | Windows Services, console jobs, SSIS, configurable file paths (no dedicated file server required; see §3.6) |

---

## 3. Tier Descriptions

### 3.1 Client Tier

- Users access TRMS via **HTTPS** (recommended in production).
- Browser-based; no thick client. Session and forms-based authentication are used.

### 3.2 Web Tier (Presentation) — DMZ

- **Placement:** Web servers are deployed **inside the DMZ** (demilitarized zone) to isolate public-facing traffic from the internal application and data tiers.
- **Platform:** Microsoft Internet Information Services (IIS).
- **Application:** MvcApplication1 (ASP.NET MVC 5, .NET 4.8) — presentation layer only; calls application tier for business logic.
- **Responsibilities:**
  - Host the MVC application and static content (CSS, JS, images, fonts).
  - SSL/TLS termination at this tier.
  - Forward authenticated requests to the application tier (BLL).
- **HA:** Multiple IIS nodes behind a **load balancer**; affinity (sticky session) recommended if session state is in-process, or use external session store for stateless scale-out.

### 3.3 Application Tier (Business Logic) — Dedicated & Load Balanced

- **Deployment model:** The **BLL (Business Logic Layer) is deployed on dedicated, load-balanced application servers**, separate from the web tier. The web tier (IIS in DMZ) communicates with the application tier; they are not co-hosted in the same IIS application pool for production/MOC-aligned deployments.
- **Components:** BLL (Business Logic Layer), Membership/Role providers, DLL (data access). Application tier hosts the business rules, authentication validation, reporting logic, audit trail, and integration with data tier.
- **Responsibilities:** Authentication (Membership/Role providers), business rules, reporting logic, audit trail, integration with DLL and SQL Server.
- **HA:** **Application servers are load balanced**; redundant app server nodes; load balancer health checks; optional external session state (e.g. SQL Server or Redis) for stateless scale-out.
- **Note:** For smaller or legacy deployments, co-location of web and app on the same IIS pool is possible; for MOC-aligned and scale-out scenarios, **dedicated load-balanced application servers** are the target architecture.

### 3.4 Data Tier

- **RDBMS:** Microsoft SQL Server.
- **Databases:**
  - **TRMS** — Primary application database (employees, attendance, leaves, payroll, audit, terminals, etc.).
  - **UNIS_*** — Integration databases (e.g. UNIS_DUHS, UNIS_DMC) for UNIS-related data.
- **Access:** Application uses connection strings (e.g. TimeTune, TimeTuneR2, UNISContext) with SQL authentication; connection timeouts configurable (e.g. TimeTuneDBTimeOut, UNISDBTimeOut in appSettings).
- **HA (MOC-aligned):**
  - **High Availability:** SQL Server Always On Availability Groups or Failover Cluster Instance (FCI) for automatic failover.
  - **Backup:** Regular full, differential, and transaction log backups; backup verification and restore drills.
  - **Redundancy:** Separate DB servers (primary + replica(s)) as per MOC/organizational policy.

### 3.6 File Storage (No Dedicated File Server)

TRMS does **not** require a dedicated file server. File storage is as follows:

- **Location:** Files (uploads, CSV imports/exports, UNIS photos, TRMS real photos, etc.) are stored on **configurable paths** on the application tier or on shared storage accessible to application and job servers.
- **Configuration:** Paths are set via **appSettings** (e.g. UNIS photos path, TRMS real photos path). These can point to local disks on app servers or to a network share if high availability or central backup is required.
- **Recommendation:** For HA, use a resilient share (e.g. clustered file server or NAS) referenced by appSettings; for single-server or small deployments, local paths on the application server(s) are sufficient.

### 3.7 Integration / Jobs Tier

- **Windows Services:** e.g. AttendancePipeLine — processes raw attendance (e.g. HaTransit) into persistent/consolidated logs.
- **Console / Scheduled Jobs:** MarkAttendance, EBI-FetchJob, UNIS-UNIS-FetchJob, UNIS-UNIS-SyncJob, CSV-* jobs, SendAutomaticEmail, ServiceOnOff, etc.
- **SSIS:** Alpeta_TRMS_UserIntegration for data integration.
- **File System:** Uses the same configurable paths as in §3.6 (no dedicated file server).
- **HA:** Jobs can be run on dedicated servers; critical jobs can be duplicated on a standby node or orchestrated via a scheduler (e.g. Windows Task Scheduler or enterprise job scheduler) with failover.

---

## 4. Software / Framework / Libraries Specification

The following table lists the main software, frameworks, and libraries required for TRMS deployment. It should be used for environment sizing and MOC compliance.

| Category | Component | Version / Notes |
|----------|-----------|------------------|
| **Web Server** | Microsoft Internet Information Services (IIS) | IIS 10 or later (Windows Server); Application Pool: .NET CLR v4.0 |
| **Runtime** | .NET Framework | 4.8 (current TRMS stack) |
| **Application** | ASP.NET MVC | 5.3 |
| **Application** | ASP.NET Web API | 5.3 |
| **ORM** | Entity Framework | 6.4.4 |
| **Database** | Microsoft SQL Server | As per MOC/organizational standard (e.g. 2019+); TRMS + UNIS_* databases |
| **Auth** | ASP.NET Forms Authentication | Built-in; custom Membership/Role providers |
| **Libraries** | EPPlus, iTextSharp, Rotativa, BouncyCastle, DotNetOpenAuth, gRPC | As referenced in solution |
| **OS** | Windows Server | As per MOC policy (e.g. 2019/2022) |
| **Load Balancer** | Hardware or software LB | For Web tier (DMZ) and Application tier |
| **Optional** | Redis / SQL Server Session State | For out-of-process session store (scale-out) |

*Future stack: see Application Architecture document for .NET Core 8+ upgrade plan.*

---

## 5. UAT Environment Specification and Architecture

- **Purpose:** User Acceptance Testing (UAT) mirrors production topology at reduced scale where appropriate, to validate functionality, integration, and security before go-live.
- **Architecture:** Same N-tier layout as production:
  - **Web tier (DMZ):** At least one IIS node (or load-balanced pair) in DMZ.
  - **Application tier:** At least one (or load-balanced) application server(s) hosting BLL.
  - **Data tier:** Dedicated UAT SQL Server instance(s) with TRMS_UAT and UNIS_UAT (or equivalent) databases; no shared data with production.
  - **Jobs/Integration:** Dedicated UAT job servers or scheduled jobs pointing to UAT databases and UAT file paths.
- **Specification:** UAT should use the same software/framework versions as in §4 (IIS, .NET 4.8, SQL Server, etc.) to avoid drift. Hardware can be scaled down (e.g. fewer nodes, smaller VM sizes) as per organizational policy.
- **Data:** UAT data is sanitized/anonymized copy of production or synthetic data; refresh and retention follow organizational policy.
- **Access:** UAT access is restricted to testers and stakeholders; network segmentation and firewall rules align with MOC/security standards.

---

## 6. High Availability (HA) Summary

- **Web/App tier:** Multiple IIS nodes + load balancer; optional external session/store for stateless scale-out.
- **Data tier:** SQL Server HA (Always On AG or FCI); regular backups and tested restore.
- **Integration/Jobs:** Dedicated or shared job servers; restart and monitoring; optional standby for critical jobs.

---

## 7. Logical Diagram (N-Tier + HA)

The diagram shows **Web tier in DMZ**, **dedicated load-balanced Application tier**, and **Data** plus **Integration/Jobs** tiers. Application servers are explicitly shown and load balanced.

```
                         ┌─────────────────────────┐
                         │   Load Balancer (Web)   │
                         └────────────┬────────────┘
                                      │
              ┌───────────────────────┼───────────────────────┐
              │         DMZ           │                       │
              ▼                       ▼                       ▼
     ┌─────────────────┐   ┌─────────────────┐   ┌─────────────────┐
     │  Web Server 1    │   │  Web Server 2    │   │  Web Server N   │
     │  IIS + MVC      │   │  IIS + MVC      │   │  IIS + MVC      │
     │  (presentation) │   │  (presentation) │   │  (presentation) │
     └────────┬────────┘   └────────┬────────┘   └────────┬────────┘
              │                     │                     │
              └─────────────────────┼─────────────────────┘
                                    │
                         ┌──────────▼──────────┐
                         │ Load Balancer (App) │
                         └──────────┬──────────┘
                                    │
              ┌─────────────────────┼─────────────────────┐
              ▼                     ▼                     ▼
     ┌─────────────────┐   ┌─────────────────┐   ┌─────────────────┐
     │ App Server 1    │   │ App Server 2    │   │ App Server N    │
     │ BLL + DLL       │   │ BLL + DLL       │   │ BLL + DLL       │
     │ (business logic)│   │ (business logic)│   │ (business logic)│
     └────────┬────────┘   └────────┬────────┘   └────────┬────────┘
              │                     │                     │
              └─────────────────────┼─────────────────────┘
                                    │
                                    ▼
                         ┌─────────────────┐
                         │   Data Tier     │
                         │  SQL Server     │
                         │  (TRMS, UNIS)   │
                         │  HA: AG / FCI   │
                         └────────┬────────┘
                                    │
              ┌─────────────────────┼─────────────────────┐
              ▼                     ▼                     ▼
     ┌─────────────────┐   ┌─────────────────┐   ┌─────────────────┐
     │ AttendancePipe  │   │  Fetch/Sync Jobs │   │  CSV / SSIS     │
     │ (Windows Svc)   │   │  (Console)       │   │  (Integration)  │
     └─────────────────┘   └─────────────────┘   └─────────────────┘
```

---

## 8. MOC Alignment Notes

- **N-tier:** Clear separation of Web, Application, Data, and Integration/Jobs tiers.
- **HA:** Redundant web/app servers and SQL Server HA (e.g. Always On or clustering) support availability and disaster recovery.
- **Security:** Application and database access use principle of least privilege; security architecture (IAM, access control, audit, encryption) is described in the Security Architecture document.
- **Operational:** Backup, monitoring, and patch management should follow organizational and MOC guidelines.

---

*This document describes the target infrastructure model for TRMS in line with HA and N-tier practices. Specific deployment details (hostnames, IPs, cluster config) should be maintained in operational runbooks and change management.*
